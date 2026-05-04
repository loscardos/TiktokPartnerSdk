using System.Text.Json;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Errors;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.RateLimiting;
using TikTokPartnerSdk.Core.Crypto;

namespace TikTokPartnerSdk.Core.Http;

public sealed class TikTokPartnerClient(
    HttpClient httpClient,
    IOptions<TikTokPartnerOptions> options,
    TikTokRequestSigner signer,
    TikTokRequestUriBuilder uriBuilder,
    TikTokRequestContentFactory contentFactory,
    ITikTokRateLimiter rateLimiter,
    TikTokResponseParser responseParser) : ITikTokPartnerClient
{
    private readonly TikTokPartnerOptions _options = options.Value;

    public async Task<TikTokPartnerResponseEnvelope<TResponse>> SendAsync<TResponse>(
        TikTokPartnerRequest request,
        CancellationToken cancellationToken)
    {
        await rateLimiter.WaitAsync(request.Authorization?.AppKey ?? _options.AppKey, cancellationToken);

        var query = request.Query.ToDictionary(
            static x => x.Key,
            static x => ConvertToQueryValue(x.Value),
            StringComparer.Ordinal);

        query["app_key"] = _options.AppKey;
        query["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture);

        var bodyText = request.Body is null ? null : TikTokRequestBodySerializer.Serialize(request.Body);
        query["sign"] = signer.Sign(
            _options.AppSecret,
            request.Path,
            query,
            bodyText);

        for (var attempt = 0; ; attempt++)
        {
            var uri = uriBuilder.Build(_options, request.Path, query);
            using var httpRequest = new HttpRequestMessage(request.Method, uri)
            {
                Content = contentFactory.Create(request.Body)
            };

            httpRequest.Headers.UserAgent.ParseAdd(_options.UserAgent);
            if (!string.IsNullOrWhiteSpace(request.AccessToken))
            {
                httpRequest.Headers.Add("x-tts-access-token", request.AccessToken);
            }

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

    private static string? ConvertToQueryValue(object? value)
    {
        return value switch
        {
            null => null,
            string text => text,
            bool boolean => boolean ? "true" : "false",
            IFormattable formattable => formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            _ => JsonSerializer.Serialize(value, new JsonSerializerOptions(JsonSerializerDefaults.Web))
        };
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
