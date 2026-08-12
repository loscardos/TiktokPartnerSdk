using System.Text.Json;
using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;
using Loscardos.TikTokPartnerSdk.Core.Webhooks;

namespace Loscardos.TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookIdempotencyKeyFactoryTests
{
    [Fact]
    public void Create_UsesNotificationIdWhenPresent()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"7327112393057371910","shop_id":"7494049642642441621","timestamp":1700000000,"data":{}}""";
        var webhookEvent = Deserialize(rawBody);
        var factory = new TikTokWebhookIdempotencyKeyFactory();

        factory.Create(webhookEvent, rawBody)
            .Should().Be("tiktok:shop:7494049642642441621:type:1:notification:7327112393057371910");
    }

    [Fact]
    public void Create_UsesSellerOpenIdWhenShopIdIsMissing()
    {
        const string rawBody = """{"type":35,"tts_notification_id":"n35","seller_open_id":"seller-open","timestamp":1700000000,"data":{}}""";
        var webhookEvent = Deserialize(rawBody);
        var factory = new TikTokWebhookIdempotencyKeyFactory();

        factory.Create(webhookEvent, rawBody)
            .Should().Be("tiktok:seller:seller-open:type:35:notification:n35");
    }

    [Fact]
    public void Create_UsesRawBodyHashFallbackWhenNotificationIdIsMissing()
    {
        const string rawBody = """{"type":68,"shop_id":"7494049642642441621","timestamp":1700000000,"data":{"sku_id":"1"}}""";
        var webhookEvent = Deserialize(rawBody);
        var factory = new TikTokWebhookIdempotencyKeyFactory();

        factory.Create(webhookEvent, rawBody)
            .Should().StartWith("tiktok:shop:7494049642642441621:type:68:timestamp:1700000000:");
    }

    private static TikTokWebhookEvent Deserialize(string rawBody)
        => JsonSerializer.Deserialize<TikTokWebhookEvent>(rawBody, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
}
