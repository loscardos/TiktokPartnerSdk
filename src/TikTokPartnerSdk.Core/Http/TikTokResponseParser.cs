using System.Text.Json;
using Loscardos.TikTokPartnerSdk.Abstractions.Errors;
using Loscardos.TikTokPartnerSdk.Abstractions.Http;

namespace Loscardos.TikTokPartnerSdk.Core.Http;

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

        envelope = NormalizeRequestId(json, envelope);

        if (envelope.Code != 0)
        {
            throw new TikTokApiException(
                envelope.Code,
                envelope.Message,
                envelope.RequestId,
                TikTokErrorClassifier.Classify(envelope.Code, envelope.Message));
        }

        return envelope;
    }

    private static TikTokPartnerResponseEnvelope<TResponse> NormalizeRequestId<TResponse>(
        string json,
        TikTokPartnerResponseEnvelope<TResponse> envelope)
    {
        if (!string.IsNullOrWhiteSpace(envelope.RequestId))
        {
            return envelope;
        }

        using var document = JsonDocument.Parse(json);
        if (document.RootElement.TryGetProperty("requestId", out var requestId)
            && requestId.ValueKind == JsonValueKind.String)
        {
            return envelope with { RequestId = requestId.GetString() };
        }

        return envelope;
    }
}
