using System.Text.Json;
using TikTokPartnerSdk.Abstractions.Http;

namespace TikTokPartnerSdk.Core.Http;

public sealed class TikTokResponseParser
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TikTokPartnerResponseEnvelope<TResponse> Parse<TResponse>(string json)
    {
        var envelope = JsonSerializer.Deserialize<TikTokPartnerResponseEnvelope<TResponse>>(json, SerializerOptions);
        if (envelope is null)
        {
            throw new InvalidOperationException("TikTok API response could not be parsed.");
        }

        return envelope;
    }
}
