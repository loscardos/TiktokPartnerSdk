namespace TikTokPartnerSdk.Abstractions.Webhooks;

public interface ITikTokWebhookRawPayloadSink
{
    Task SaveAsync(TikTokWebhookEnvelope envelope, CancellationToken cancellationToken);
}
