using System.Text.Json;
using TikTokPartnerSdk.Abstractions.Webhooks;

namespace TikTokPartnerSdk.Core.Webhooks;

public sealed class TikTokWebhookParser(
    ITikTokWebhookSignatureVerifier signatureVerifier,
    TikTokWebhookTimestampValidator timestampValidator,
    ITikTokWebhookIdempotencyKeyFactory idempotencyKeyFactory) : ITikTokWebhookParser
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public TikTokWebhookReceiveResult TryReceive(
        string rawBody,
        string signature,
        DateTimeOffset receivedAt)
        => TryReceive(string.Empty, rawBody, signature, receivedAt);

    public TikTokWebhookReceiveResult TryReceive(
        string path,
        string rawBody,
        string signature,
        DateTimeOffset receivedAt)
    {
        if (!signatureVerifier.Verify(path, rawBody, signature))
        {
            return TikTokWebhookReceiveResult.Reject("invalid_signature");
        }

        var signedTimestamp = signatureVerifier.GetSignedTimestamp(signature);
        if (signedTimestamp is not null && !timestampValidator.IsFresh(signedTimestamp, receivedAt))
        {
            return TikTokWebhookReceiveResult.Reject("stale_signature_timestamp");
        }

        TikTokWebhookEvent? webhookEvent;
        try
        {
            webhookEvent = JsonSerializer.Deserialize<TikTokWebhookEvent>(rawBody, JsonOptions);
        }
        catch (JsonException)
        {
            return TikTokWebhookReceiveResult.Reject("invalid_json");
        }

        if (webhookEvent is null)
        {
            return TikTokWebhookReceiveResult.Reject("empty_payload");
        }

        if (!timestampValidator.IsFresh(webhookEvent, receivedAt))
        {
            return TikTokWebhookReceiveResult.Reject("stale_timestamp");
        }

        var envelope = new TikTokWebhookEnvelope(
            Marketplace: "tiktok",
            RawBody: rawBody,
            Signature: signature,
            IdempotencyKey: idempotencyKeyFactory.Create(webhookEvent, rawBody),
            ReceivedAt: receivedAt,
            Event: webhookEvent);

        return TikTokWebhookReceiveResult.Accept(envelope);
    }

    public TikTokWebhookTypedEvent<TData>? TryParseData<TData>(TikTokWebhookEnvelope envelope)
    {
        try
        {
            var data = envelope.Event.Data.Deserialize<TData>(JsonOptions);
            return data is null ? null : new TikTokWebhookTypedEvent<TData>(envelope, data);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
