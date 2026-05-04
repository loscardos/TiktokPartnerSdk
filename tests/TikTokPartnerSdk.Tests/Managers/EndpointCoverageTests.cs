using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Managers.Generated;

namespace TikTokPartnerSdk.Tests.Managers;

public sealed class EndpointCoverageTests
{
    [Fact]
    public void Starter_generated_interfaces_should_cover_authorization_seller_and_event_categories()
    {
        typeof(IAuthorizationApi).Should().NotBeNull();
        typeof(ISellerApi).Should().NotBeNull();
        typeof(IEventApi).Should().NotBeNull();
    }

    [Fact]
    public void Generated_interfaces_should_include_orders_and_products()
    {
        typeof(IOrderApi).Should().NotBeNull();
        typeof(IProductApi).Should().NotBeNull();
    }

    [Fact]
    public void Generated_interfaces_should_include_fulfillment_and_logistics()
    {
        typeof(IFulfillmentApi).Should().NotBeNull();
        typeof(ILogisticsApi).Should().NotBeNull();
    }

    [Fact]
    public void Generated_interfaces_should_include_return_and_refund()
    {
        typeof(IReturnAndRefundApi).Should().NotBeNull();
    }

    [Fact]
    public void Generated_interfaces_should_include_finance()
    {
        typeof(IFinanceApi).Should().NotBeNull();
    }

    [Fact]
    public void Generated_interfaces_should_include_fbt()
    {
        typeof(IFulfilledByTiktokFbtApi).Should().NotBeNull();
    }

    [Fact]
    public void Generated_interfaces_should_include_supply_chain()
    {
        typeof(ISupplyChainApi).Should().NotBeNull();
    }

    [Fact]
    public void Generated_endpoint_count_should_match_committed_schema_snapshots()
    {
        var schemaCount = Directory
            .EnumerateFiles(Path.Combine(TestPaths.RepositoryRoot, "tests", "TikTokPartnerSdk.Tests", "Fixtures", "Schemas"), "*.json")
            .Count();

        var generatedMethods = typeof(ISellerApi).Assembly
            .GetTypes()
            .Where(static type => type.Namespace == "TikTokPartnerSdk.Abstractions.Managers.Generated")
            .SelectMany(static type => type.GetMethods())
            .Count(static method => method.Name.EndsWith("Async", StringComparison.Ordinal));

        generatedMethods.Should().Be(schemaCount);
        generatedMethods.Should().BeGreaterThanOrEqualTo(146);
    }
}
