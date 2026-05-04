using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Errors;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.RateLimiting;

namespace TikTokPartnerSdk.Core.Http;

public sealed class TikTokAuthClient(
    HttpClient httpClient,
    IOptions<TikTokPartnerOptions> options,
    ITikTokRateLimiter rateLimiter,
    TikTokResponseParser responseParser) : ITikTokAuthClient
{
    private static readonly JsonSerializerOptions BodySerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly TikTokPartnerOptions _options = options.Value;

    public async Task<TikTokPartnerResponseEnvelope<TResponse>> PostAsync<TResponse>(
        string path,
        object body,
        CancellationToken cancellationToken)
    {
        await rateLimiter.WaitAsync("auth", cancellationToken);

        for (var attempt = 0; ; attempt++)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, BuildUri(path))
            {
                Content = JsonContent.Create(body, options: BodySerializerOptions)
            };

            httpRequest.Headers.UserAgent.ParseAdd(_options.UserAgent);

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(_options.RequestTimeout);

            using var response = await httpClient.SendAsync(httpRequest, timeoutCts.Token);
            var payload = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return responseParser.Parse<TResponse>(payload);
            }

            if (IsTransient(response.StatusCode) && attempt < _options.MaxTransientRetries)
            {
                await DelayAsync(attempt, cancellationToken);
                continue;
            }

            ThrowHttpException<TResponse>(response, payload);
        }
    }

    private Uri BuildUri(string path)
    {
        var baseUrl = _options.AuthApiBaseUrl.TrimEnd('/');
        var normalizedPath = path.TrimStart('/');
        return new Uri($"{baseUrl}/{normalizedPath}", UriKind.Absolute);
    }

    private async Task DelayAsync(int attempt, CancellationToken cancellationToken)
    {
        var delay = TimeSpan.FromMilliseconds(_options.RetryBaseDelay.TotalMilliseconds * Math.Pow(2, attempt));
        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay, cancellationToken);
        }
    }

    private static bool IsTransient(System.Net.HttpStatusCode statusCode)
        => statusCode is System.Net.HttpStatusCode.RequestTimeout
            or System.Net.HttpStatusCode.TooManyRequests
            or >= System.Net.HttpStatusCode.InternalServerError;

    private void ThrowHttpException<TResponse>(HttpResponseMessage response, string payload)
    {
        if (!string.IsNullOrWhiteSpace(payload))
        {
            try
            {
                responseParser.Parse<TResponse>(payload);
            }
            catch (TikTokApiException)
            {
                throw;
            }
            catch (JsonException)
            {
            }
        }

        throw new TikTokApiException(
            (int)response.StatusCode,
            response.ReasonPhrase ?? "HTTP request failed",
            null,
            TikTokErrorClassifier.Classify(response.StatusCode));
    }
}
