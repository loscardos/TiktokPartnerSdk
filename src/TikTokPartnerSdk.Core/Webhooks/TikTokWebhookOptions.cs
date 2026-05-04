namespace TikTokPartnerSdk.Core.Webhooks;

public sealed class TikTokWebhookOptions
{
    public string SignatureHeaderName { get; set; } = "authorization";

    public TimeSpan TimestampTolerance { get; set; } = TimeSpan.FromMinutes(5);
}
