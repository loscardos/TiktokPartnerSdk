using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;

namespace TikTokPartnerSdk.Tests.Configuration;

public sealed class TikTokPartnerOptionsTests
{
    [Fact]
    public void Options_should_expose_main_and_auth_base_urls()
    {
        var options = new TikTokPartnerOptions();

        options.OpenApiBaseUrl.Should().Be("https://open-api.tiktokglobalshop.com");
        options.AuthApiBaseUrl.Should().Be("https://auth.tiktok-shops.com/api/v2");
    }

    [Fact]
    public void Token_record_should_capture_token_kind_and_owner_scope()
    {
        var record = new TikTokTokenRecord(
            AccessTokenKind: TikTokAccessTokenKind.Partner,
            AccessToken: "access",
            RefreshToken: "refresh",
            ExpiresAtUtc: DateTimeOffset.UtcNow.AddHours(1),
            RefreshTokenExpiresAtUtc: DateTimeOffset.UtcNow.AddDays(30),
            ShopCipher: null,
            AppKey: "key");

        record.AccessTokenKind.Should().Be(TikTokAccessTokenKind.Partner);
        record.AppKey.Should().Be("key");
    }
}
