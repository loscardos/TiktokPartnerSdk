namespace TikTokPartnerSdk.Abstractions.Webhooks;

public interface ITikTokWebhookParser
{
    TikTokWebhookReceiveResult TryReceive(
        string rawBody,
        string signature,
        DateTimeOffset receivedAt);

    TikTokWebhookTypedEvent<TData>? TryParseData<TData>(
        TikTokWebhookEnvelope envelope);
}
