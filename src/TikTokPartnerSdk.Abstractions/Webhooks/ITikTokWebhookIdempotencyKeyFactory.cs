namespace TikTokPartnerSdk.Abstractions.Webhooks;

public interface ITikTokWebhookIdempotencyKeyFactory
{
    string Create(TikTokWebhookEvent webhookEvent, string rawBody);
}
