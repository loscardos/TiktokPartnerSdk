using FluentAssertions;
using TikTokPartnerSdk.SampleConsole;

namespace TikTokPartnerSdk.Tests.SampleConsole;

public sealed class CertificationReportTests
{
    [Fact]
    public void ToMarkdown_should_include_summary_and_rows()
    {
        var markdown = CertificationReport.ToMarkdown([
            new CertificationResult("Seller", "GET /seller/202309/shops", CertificationStatus.Pass, "shops=1"),
            new CertificationResult("Order", "GET /order/202507/orders/{order_id}", CertificationStatus.SkipNoData, "No order id from search")
        ]);

        markdown.Should().Contain("| Pass | 1 |");
        markdown.Should().Contain("| SkipNoData | 1 |");
        markdown.Should().Contain("GET /seller/202309/shops");
        markdown.Should().Contain("No order id from search");
    }
}
