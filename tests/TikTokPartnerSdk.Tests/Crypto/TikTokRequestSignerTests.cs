using FluentAssertions;
using TikTokPartnerSdk.Core.Crypto;

namespace TikTokPartnerSdk.Tests.Crypto;

public sealed class TikTokRequestSignerTests
{
    [Fact]
    public void Signer_should_return_non_empty_hmac_sha256_signature()
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

        signature.Should().NotBeNullOrWhiteSpace();
        signature.Length.Should().Be(64);
    }
}
