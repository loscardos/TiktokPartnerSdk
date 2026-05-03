using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Core.Http;

namespace TikTokPartnerSdk.Tests.Http;

public sealed class TikTokRequestUriBuilderTests
{
    [Fact]
    public void Build_should_combine_base_url_path_and_query()
    {
        var builder = new TikTokRequestUriBuilder();

        var uri = builder.Build(
            new TikTokPartnerOptions(),
            "/seller/202309/orders/search",
            new Dictionary<string, string?>
            {
                ["app_key"] = "key",
                ["timestamp"] = "1710000000"
            });

        uri.ToString().Should().Be(
            "https://open-api.tiktokglobalshop.com/seller/202309/orders/search?app_key=key&timestamp=1710000000");
    }
}
