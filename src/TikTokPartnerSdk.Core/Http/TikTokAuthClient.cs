using System.Globalization;
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
    private readonly TikTokPartnerOptions _options = options.Value;

    public async Task<TikTokPartnerResponseEnvelope<TResponse>> PostAsync<TResponse>(
        string path,
        object body,
        CancellationToken cancellationToken)
    {
        await rateLimiter.WaitAsync("auth", cancellationToken);

        for (var attempt = 0; ; attempt++)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, BuildUri(path, body));

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

    private Uri BuildUri(string path, object body)
    {
        var baseUrl = _options.AuthApiBaseUrl.TrimEnd('/');
        var normalizedPath = path.TrimStart('/');
        var query = ToQueryString(body);
        return new Uri($"{baseUrl}/{normalizedPath}{query}", UriKind.Absolute);
    }

    private static string ToQueryString(object body)
    {
        if (body is not IEnumerable<KeyValuePair<string, object?>> values)
        {
            return string.Empty;
        }

        var parts = values
            .Where(static item => item.Value is not null)
            .Select(static item =>
                $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(Convert.ToString(item.Value, CultureInfo.InvariantCulture) ?? string.Empty)}");

        var query = string.Join("&", parts);
        return query.Length == 0 ? string.Empty : "?" + query;
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
