using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Core.Webhooks;
using Loscardos.TikTokPartnerSdk.Generated.Webhooks;

namespace Loscardos.TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookParserTests
{
    [Fact]
    public void TryReceive_ReturnsAcceptedEnvelopeForValidSignatureAndFreshTimestamp()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{"order_id":"o1","order_status":"UNPAID","is_on_hold_order":false,"update_time":1700000000}}""";
        var parser = CreateParser();
        var result = parser.TryReceive(rawBody, Sign(rawBody), DateTimeOffset.FromUnixTimeSeconds(1_700_000_120));

        result.IsAccepted.Should().BeTrue();
        result.Envelope!.IdempotencyKey.Should().Be("tiktok:shop:s1:type:1:notification:n1");
        result.Envelope.Event.Type.Should().Be(TikTokWebhookType.OrderStatusChange);
    }

    [Fact]
    public void TryReceive_ReturnsRejectedResultForInvalidSignature()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{}}""";
        var parser = CreateParser();

        var result = parser.TryReceive(rawBody, "bad-signature", DateTimeOffset.FromUnixTimeSeconds(1_700_000_120));

        result.IsAccepted.Should().BeFalse();
        result.RejectionReason.Should().Be("invalid_signature");
        result.Envelope.Should().BeNull();
    }

    [Fact]
    public void TryReceive_ReturnsRejectedResultForStaleTimestamp()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{}}""";
        var parser = CreateParser();

        var result = parser.TryReceive(rawBody, Sign(rawBody), DateTimeOffset.FromUnixTimeSeconds(1_700_001_000));

        result.IsAccepted.Should().BeFalse();
        result.RejectionReason.Should().Be("stale_timestamp");
    }

    [Fact]
    public void TryReceive_UsesSignedTimestampWhenTikTokSignatureHeaderProvidesOne()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"n1","shop_id":"s1","timestamp":1800000000,"data":{}}""";
        var parser = CreateParser();

        var result = parser.TryReceive(rawBody, SignTikTokHeader(rawBody, 1_700_000_000), DateTimeOffset.FromUnixTimeSeconds(1_700_001_000));

        result.IsAccepted.Should().BeFalse();
        result.RejectionReason.Should().Be("stale_signature_timestamp");
    }

    [Fact]
    public void TryParseData_ReturnsTypedOrderStatusChangeData()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{"order_id":"o1","order_status":"UNPAID","is_on_hold_order":false,"update_time":1700000000}}""";
        var parser = CreateParser();
        var result = parser.TryReceive(rawBody, Sign(rawBody), DateTimeOffset.FromUnixTimeSeconds(1_700_000_120));

        var typed = parser.TryParseData<TikTokOrderStatusChangeWebhookData>(result.Envelope!);

        typed.Should().NotBeNull();
        typed!.Data.OrderId.Should().Be("o1");
        typed.Data.OrderStatus.Should().Be("UNPAID");
        typed.Data.IsOnHoldOrder.Should().BeFalse();
    }

    private static TikTokWebhookParser CreateParser()
        => new(
            new TikTokWebhookSignatureVerifier(Options.Create(new TikTokPartnerOptions
            {
                AppKey = "app-key",
                AppSecret = "app-secret",
                OpenApiBaseUrl = "https://open-api.tiktokglobalshop.com"
            })),
            new TikTokWebhookTimestampValidator(Options.Create(new TikTokWebhookOptions
            {
                TimestampTolerance = TimeSpan.FromMinutes(5)
            })),
            new TikTokWebhookIdempotencyKeyFactory());

    private static string Sign(string rawBody)
        => Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("app-secret"),
            Encoding.UTF8.GetBytes(rawBody))).ToLowerInvariant();

    private static string SignTikTokHeader(string rawBody, long timestamp)
    {
        var signedPayload = $"{timestamp}.{rawBody}";
        var signature = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("app-secret"),
            Encoding.UTF8.GetBytes(signedPayload))).ToLowerInvariant();
        return $"t={timestamp},s={signature}";
    }
}
