using System.Text.Json;
using System.Text.Json.Serialization;

namespace TikTokPartnerSdk.Abstractions.Webhooks;

public sealed record TikTokWebhookEvent(
    [property: JsonPropertyName("type")] long Type,
    [property: JsonPropertyName("tts_notification_id")] string? NotificationId,
    [property: JsonPropertyName("shop_id")] string? ShopId,
    [property: JsonPropertyName("seller_open_id")] string? SellerOpenId,
    [property: JsonPropertyName("creator_open_id")] string? CreatorOpenId,
    [property: JsonPropertyName("timestamp")] long? Timestamp,
    [property: JsonPropertyName("data")] JsonElement Data);
