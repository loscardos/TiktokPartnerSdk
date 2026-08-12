using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.Errors;
using Loscardos.TikTokPartnerSdk.Abstractions.Http;
using Loscardos.TikTokPartnerSdk.Abstractions.RateLimiting;

namespace Loscardos.TikTokPartnerSdk.Core.Http;

public sealed class TikTokAuthClient(
    HttpClient httpClient,
    IOptions<TikTokPartnerOptions> options,
    ITikTokRateLimiter rateLimiter,
    TikTokResponseParser responseParser) : ITikTokAuthClient
{
    private readonly TikTokPartnerOptions _options = options.Value;

    public async Task<TikTokPartnerResponseEnvelope<TResponse>> GetAsync<TResponse>(
        string path,
        object query,
        CancellationToken cancellationToken)
    {
        await rateLimiter.WaitAsync("auth", cancellationToken);

        for (var attempt = 0; ; attempt++)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, BuildUri(path, query));

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

    private Uri BuildUri(string path, object query)
    {
        var baseUrl = _options.AuthApiBaseUrl.TrimEnd('/');
        var normalizedPath = path.TrimStart('/');
        var queryString = ToQueryString(query);
        return new Uri($"{baseUrl}/{normalizedPath}{queryString}", UriKind.Absolute);
    }

    private static string ToQueryString(object query)
    {
        if (query is not IEnumerable<KeyValuePair<string, object?>> values)
        {
            return string.Empty;
        }

        var parts = values
            .Where(static item => item.Value is not null)
            .Select(static item =>
                $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(Convert.ToString(item.Value, CultureInfo.InvariantCulture) ?? string.Empty)}");

        var queryString = string.Join("&", parts);
        return queryString.Length == 0 ? string.Empty : "?" + queryString;
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
