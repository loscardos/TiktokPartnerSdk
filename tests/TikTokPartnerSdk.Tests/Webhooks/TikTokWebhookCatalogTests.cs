using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Generated.Webhooks;

namespace Loscardos.TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookCatalogTests
{
    [Fact]
    public void Catalog_ShouldContainAllKnownWebhookTypes()
    {
        TikTokWebhookCatalog.All.Should().HaveCount(39);
        TikTokWebhookCatalog.All.Select(item => item.Name).Should().Contain([
            "Order status change",
            "Product status change",
            "Seller deauthorization",
            "Shoppable content posting"
        ]);
    }

    [Fact]
    public void Catalog_ShouldFindOrderStatusChangeByType()
    {
        var item = TikTokWebhookCatalog.FindByType(TikTokWebhookType.OrderStatusChange);

        item.Should().NotBeNull();
        item!.Type.Should().Be(1);
        item.Name.Should().Be("Order status change");
        item.Category.Should().Be("Order");
        item.Fields.Should().Contain(field => field.Path == "order_status");
    }

    [Fact]
    public void Catalog_ShouldFindShoppableContentPostingByType()
    {
        var item = TikTokWebhookCatalog.FindByType(TikTokWebhookType.ShoppableContentPosting);

        item.Should().NotBeNull();
        item!.Type.Should().Be(17);
        item.Category.Should().Be("Affiliate Creator");
        item.Fields.Should().Contain(field => field.Path == "data.event.type");
    }
}
