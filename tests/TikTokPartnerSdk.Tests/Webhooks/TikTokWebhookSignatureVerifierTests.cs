using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Core.Webhooks;

namespace TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookSignatureVerifierTests
{
    [Fact]
    public void Verify_ReturnsTrueForValidRawBodyHmacSha256Signature()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{}}""";
        var signature = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("app-secret"),
            Encoding.UTF8.GetBytes(rawBody))).ToLowerInvariant();
        var verifier = CreateVerifier();

        verifier.Verify(rawBody, signature).Should().BeTrue();
    }

    [Fact]
    public void Verify_ReturnsTrueForTikTokShopWebhookAuthorizationSignature()
    {
        const string rawBody = """{"type":15,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{"product_id":123}}""";
        var signature = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("app-secret"),
            Encoding.UTF8.GetBytes($"app-key{rawBody}"))).ToLowerInvariant();
        var verifier = CreateVerifier();

        verifier.Verify("/webhooks/tiktok", rawBody, signature).Should().BeTrue();
    }

    [Fact]
    public void Verify_UsesWebhookSecretWhenConfigured()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{}}""";
        var signature = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("webhook-secret"),
            Encoding.UTF8.GetBytes($"app-key{rawBody}"))).ToLowerInvariant();
        var verifier = new TikTokWebhookSignatureVerifier(Options.Create(new TikTokPartnerOptions
        {
            AppKey = "app-key",
            AppSecret = "app-secret",
            WebhookSecret = "webhook-secret"
        }));

        verifier.Verify(rawBody, signature).Should().BeTrue();
    }

    [Fact]
    public void Verify_ReturnsTrueForOfficialWebhookSignatureWithPath()
    {
        const string rawBody = """{"type":15,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{"product_id":123}}""";
        const string path = "/webhooks/tiktok";
        var signature = SignOfficialWebhook(path, rawBody, "app-secret");
        var verifier = CreateVerifier();

        verifier.Verify(path, rawBody, signature).Should().BeTrue();
    }

    [Fact]
    public void Verify_ReturnsFalseForMissingSignature()
    {
        var verifier = CreateVerifier();

        verifier.Verify("""{"type":1}""", "").Should().BeFalse();
    }

    [Fact]
    public void Verify_ReturnsFalseForDifferentPayload()
    {
        const string rawBody = """{"type":1}""";
        var signature = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("app-secret"),
            Encoding.UTF8.GetBytes("""{"type":2}"""))).ToLowerInvariant();
        var verifier = CreateVerifier();

        verifier.Verify(rawBody, signature).Should().BeFalse();
    }

    [Fact]
    public void Verify_ReturnsTrueForTikTokSignatureHeader()
    {
        const string rawBody = """{"type":1,"tts_notification_id":"n1","shop_id":"s1","timestamp":1700000000,"data":{}}""";
        var signature = SignTikTokHeader(rawBody, 1_700_000_120);
        var verifier = CreateVerifier();

        verifier.Verify(rawBody, signature).Should().BeTrue();
    }

    [Fact]
    public void GetSignedTimestamp_ReturnsTimestampFromTikTokSignatureHeader()
    {
        const string rawBody = """{"type":1,"data":{}}""";
        var signature = SignTikTokHeader(rawBody, 1_700_000_120);
        var verifier = CreateVerifier();

        verifier.GetSignedTimestamp(signature).Should().Be(1_700_000_120);
    }

    [Fact]
    public void Verify_ReturnsFalseForTikTokSignatureHeaderWithDifferentPayload()
    {
        var signature = SignTikTokHeader("""{"type":2}""", 1_700_000_120);
        var verifier = CreateVerifier();

        verifier.Verify("""{"type":1}""", signature).Should().BeFalse();
    }

    private static TikTokWebhookSignatureVerifier CreateVerifier()
        => new(Options.Create(new TikTokPartnerOptions
        {
            AppKey = "app-key",
            AppSecret = "app-secret",
            OpenApiBaseUrl = "https://open-api.tiktokglobalshop.com"
        }));

    private static string SignTikTokHeader(string rawBody, long timestamp)
    {
        var signedPayload = $"{timestamp}.{rawBody}";
        var signature = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("app-secret"),
            Encoding.UTF8.GetBytes(signedPayload))).ToLowerInvariant();
        return $"t={timestamp},s={signature}";
    }

    private static string SignOfficialWebhook(string path, string rawBody, string secret)
    {
        var input = $"{secret}{path}{rawBody}{secret}";
        return Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(secret),
            Encoding.UTF8.GetBytes(input))).ToLowerInvariant();
    }
}
