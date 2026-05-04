namespace TikTokPartnerSdk.Abstractions.Webhooks;

public sealed record TikTokWebhookTypedEvent<TData>(
    TikTokWebhookEnvelope Envelope,
    TData Data);
