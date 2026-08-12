using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;

namespace Loscardos.TikTokPartnerSdk.Core.Webhooks;

public sealed class TikTokWebhookTimestampValidator(IOptions<TikTokWebhookOptions> options)
{
    private readonly TikTokWebhookOptions _options = options.Value;

    public bool IsFresh(TikTokWebhookEvent webhookEvent, DateTimeOffset receivedAt)
        => IsFresh(webhookEvent.Timestamp, receivedAt);

    public bool IsFresh(long? unixTimestamp, DateTimeOffset receivedAt)
    {
        if (unixTimestamp is null)
        {
            return false;
        }

        var sentAt = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp.Value);
        var delta = (receivedAt - sentAt).Duration();
        return delta <= _options.TimestampTolerance;
    }
}
