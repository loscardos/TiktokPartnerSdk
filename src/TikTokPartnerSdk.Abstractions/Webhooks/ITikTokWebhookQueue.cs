namespace TikTokPartnerSdk.Abstractions.Webhooks;

public interface ITikTokWebhookQueue
{
    Task EnqueueAsync(TikTokWebhookEnvelope envelope, CancellationToken cancellationToken);
}
