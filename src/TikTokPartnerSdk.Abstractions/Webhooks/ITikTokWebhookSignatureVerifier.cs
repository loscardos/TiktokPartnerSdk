namespace TikTokPartnerSdk.Abstractions.Webhooks;

public interface ITikTokWebhookSignatureVerifier
{
    bool Verify(string rawBody, string signature);
}
