using System.Text.Json;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Core.Crypto;

namespace TikTokPartnerSdk.Core.Http;

public sealed class TikTokPartnerClient(
    HttpClient httpClient,
    IOptions<TikTokPartnerOptions> options,
    TikTokRequestSigner signer,
    TikTokRequestUriBuilder uriBuilder,
    TikTokRequestContentFactory contentFactory,
    TikTokResponseParser responseParser) : ITikTokPartnerClient
{
    private static readonly JsonSerializerOptions BodySerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly TikTokPartnerOptions _options = options.Value;

    public async Task<TikTokPartnerResponseEnvelope<TResponse>> SendAsync<TResponse>(
        TikTokPartnerRequest request,
        CancellationToken cancellationToken)
    {
        var query = request.Query.ToDictionary(
            static x => x.Key,
            static x => ConvertToQueryValue(x.Value),
            StringComparer.Ordinal);

        query["app_key"] = _options.AppKey;
        query["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture);

        var bodyText = request.Body is null ? null : JsonSerializer.Serialize(request.Body, BodySerializerOptions);
        query["sign"] = signer.Sign(
            _options.AppSecret,
            request.Path,
            query,
            bodyText);

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
        return responseParser.Parse<TResponse>(payload);
    }

    private static string? ConvertToQueryValue(object? value)
    {
        return value switch
        {
            null => null,
            string text => text,
            bool boolean => boolean ? "true" : "false",
            IFormattable formattable => formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            _ => JsonSerializer.Serialize(value, BodySerializerOptions)
        };
    }
}
