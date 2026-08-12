using System.Security.Cryptography;
using System.Text;
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;

namespace Loscardos.TikTokPartnerSdk.Core.Webhooks;

public sealed class TikTokWebhookIdempotencyKeyFactory : ITikTokWebhookIdempotencyKeyFactory
{
    public string Create(TikTokWebhookEvent webhookEvent, string rawBody)
    {
        var owner = GetOwner(webhookEvent);

        if (!string.IsNullOrWhiteSpace(webhookEvent.NotificationId))
        {
            return $"tiktok:{owner}:type:{webhookEvent.Type}:notification:{webhookEvent.NotificationId}";
        }

        var timestamp = webhookEvent.Timestamp?.ToString() ?? "none";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawBody))).ToLowerInvariant();
        return $"tiktok:{owner}:type:{webhookEvent.Type}:timestamp:{timestamp}:{hash}";
    }

    private static string GetOwner(TikTokWebhookEvent webhookEvent)
    {
        if (!string.IsNullOrWhiteSpace(webhookEvent.ShopId))
        {
            return $"shop:{webhookEvent.ShopId}";
        }

        if (!string.IsNullOrWhiteSpace(webhookEvent.SellerOpenId))
        {
            return $"seller:{webhookEvent.SellerOpenId}";
        }

        if (!string.IsNullOrWhiteSpace(webhookEvent.CreatorOpenId))
        {
            return $"creator:{webhookEvent.CreatorOpenId}";
        }

        return "unknown";
    }
}
