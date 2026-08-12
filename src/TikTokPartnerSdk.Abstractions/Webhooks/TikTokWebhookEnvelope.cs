namespace Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;

public sealed record TikTokWebhookEnvelope(
    string Marketplace,
    string RawBody,
    string Signature,
    string IdempotencyKey,
    DateTimeOffset ReceivedAt,
    TikTokWebhookEvent Event);
