namespace Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;

public interface ITikTokWebhookSignatureVerifier
{
    bool Verify(string rawBody, string signature);

    bool Verify(string path, string rawBody, string signature);

    long? GetSignedTimestamp(string signature);
}
