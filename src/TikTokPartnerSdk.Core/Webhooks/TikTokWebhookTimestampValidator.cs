using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Webhooks;

namespace TikTokPartnerSdk.Core.Webhooks;

public sealed class TikTokWebhookTimestampValidator(IOptions<TikTokWebhookOptions> options)
{
    private readonly TikTokWebhookOptions _options = options.Value;

    public bool IsFresh(TikTokWebhookEvent webhookEvent, DateTimeOffset receivedAt)
    {
        if (webhookEvent.Timestamp is null)
        {
            return false;
        }

        var sentAt = DateTimeOffset.FromUnixTimeSeconds(webhookEvent.Timestamp.Value);
        var delta = (receivedAt - sentAt).Duration();
        return delta <= _options.TimestampTolerance;
    }
}
