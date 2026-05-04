using FluentAssertions;
using TikTokPartnerSdk.Core.Crypto;

namespace TikTokPartnerSdk.Tests.Crypto;

public sealed class TikTokRequestSignatureSemanticsTests
{
    [Fact]
    public void Signer_should_be_stable_for_same_input()
    {
        var signer = new TikTokRequestSigner();
        var query = new Dictionary<string, string?>
        {
            ["app_key"] = "key",
            ["timestamp"] = "1710000000"
        };

        var first = signer.Sign("secret", "/authorization/202309/shops", query, null);
        var second = signer.Sign("secret", "/authorization/202309/shops", query, null);

        first.Should().Be(second);
    }
}
