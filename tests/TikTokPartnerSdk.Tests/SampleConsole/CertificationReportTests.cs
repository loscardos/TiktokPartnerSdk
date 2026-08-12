using FluentAssertions;
using Loscardos.TikTokPartnerSdk.SampleConsole;

namespace Loscardos.TikTokPartnerSdk.Tests.SampleConsole;

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

    [Fact]
    public void Endpoint_matrix_should_classify_generated_endpoints_and_mark_certified_rows()
    {
        var endpoints = GeneratedEndpointInventory.ReadFromDirectory(Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Core",
            "Managers",
            "Generated"));

        endpoints.Should().HaveCount(271);

        var rows = EndpointCertificationMatrix.Create(
            endpoints,
            ReadonlyCertificationCatalog.CertifiedEndpoints);

        rows.Should().Contain(row =>
            row.Area == "Order"
            && row.Method == "POST"
            && row.Path == "/order/202309/orders/search"
            && row.Classification == EndpointClassification.Readonly
            && row.Status == EndpointCertificationStatus.ReadonlyCertified);

        rows.Should().Contain(row =>
            row.Area == "Product"
            && row.Method == "POST"
            && row.Path == "/product/202309/products"
            && row.Classification == EndpointClassification.Mutation
            && row.Status == EndpointCertificationStatus.MutationNeedsFixture);
    }

    [Fact]
    public void Endpoint_matrix_markdown_should_include_summary_and_fixture_columns()
    {
        var markdown = EndpointCertificationMatrix.ToMarkdown([
            new EndpointCertificationRow(
                "Product",
                "POST",
                "/product/202309/products",
                "seller",
                EndpointClassification.Mutation,
                EndpointCertificationStatus.MutationNeedsFixture,
                RequiresShopCipher: true,
                RequiresPathId: false,
                RequiresFixture: true,
                "write endpoint needs opt-in sandbox fixture")
        ]);

        markdown.Should().Contain("# TikTok Partner SDK Endpoint Certification Matrix");
        markdown.Should().Contain("| MutationNeedsFixture | 1 |");
        markdown.Should().Contain("| Product | POST | /product/202309/products | seller | Mutation | MutationNeedsFixture | yes | no | yes | write endpoint needs opt-in sandbox fixture |");
    }
}
