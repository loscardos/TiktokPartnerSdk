using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Core.Http;

namespace Loscardos.TikTokPartnerSdk.Tests.Http;

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

    [Fact]
    public void Build_should_omit_null_query_values()
    {
        var builder = new TikTokRequestUriBuilder();

        var uri = builder.Build(
            new TikTokPartnerOptions(),
            "/finance/202309/statements",
            new Dictionary<string, string?>
            {
                ["payment_status"] = null,
                ["page_size"] = "10"
            });

        uri.ToString().Should().Be(
            "https://open-api.tiktokglobalshop.com/finance/202309/statements?page_size=10");
    }
}
