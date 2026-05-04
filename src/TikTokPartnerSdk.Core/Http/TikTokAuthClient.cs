using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Http;

namespace TikTokPartnerSdk.Core.Http;

public sealed class TikTokAuthClient(
    HttpClient httpClient,
    IOptions<TikTokPartnerOptions> options,
    TikTokResponseParser responseParser) : ITikTokAuthClient
{
    private static readonly JsonSerializerOptions BodySerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly TikTokPartnerOptions _options = options.Value;

    public async Task<TikTokPartnerResponseEnvelope<TResponse>> PostAsync<TResponse>(
        string path,
        object body,
        CancellationToken cancellationToken)
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
        return responseParser.Parse<TResponse>(payload);
    }

    private Uri BuildUri(string path)
    {
        var baseUrl = _options.AuthApiBaseUrl.TrimEnd('/');
        var normalizedPath = path.TrimStart('/');
        return new Uri($"{baseUrl}/{normalizedPath}", UriKind.Absolute);
    }
}
