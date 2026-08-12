using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Core.Crypto;

namespace Loscardos.TikTokPartnerSdk.Tests.Crypto;

public sealed class TikTokRequestSignerTests
{
    [Fact]
    public void Signer_should_match_tiktok_hmac_sha256_signature()
    {
        var signer = new TikTokRequestSigner();

        var signature = signer.Sign(
            appSecret: "secret",
            path: "/authorization/202309/shops",
            query: new Dictionary<string, string?>
            {
                ["app_key"] = "key",
                ["timestamp"] = "1710000000"
            },
            body: null);

        signature.Should().Be("19b4893e129217f43b56b019acb6630a0ece2d3814b01c7b0b63da444c8ce030");
    }

    [Fact]
    public void Signer_should_exclude_signature_and_access_token_from_canonical_query()
    {
        var signer = new TikTokRequestSigner();
        var query = new Dictionary<string, string?>
        {
            ["app_key"] = "key",
            ["timestamp"] = "1710000000",
            ["sign"] = "ignored",
            ["access_token"] = "ignored"
        };

        var signature = signer.Sign("secret", "/authorization/202309/shops", query, null);

        signature.Should().Be("19b4893e129217f43b56b019acb6630a0ece2d3814b01c7b0b63da444c8ce030");
    }
}
