namespace TikTokPartnerSdk.Abstractions.Webhooks;

public sealed record TikTokWebhookReceiveResult(
    bool IsAccepted,
    string? RejectionReason,
    TikTokWebhookEnvelope? Envelope)
{
    public static TikTokWebhookReceiveResult Accept(TikTokWebhookEnvelope envelope)
        => new(true, null, envelope);

    public static TikTokWebhookReceiveResult Reject(string reason)
        => new(false, reason, null);
}
