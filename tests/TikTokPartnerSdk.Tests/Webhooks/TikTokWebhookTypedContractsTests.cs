using FluentAssertions;
using TikTokPartnerSdk.Generated.Webhooks;

namespace TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookTypedContractsTests
{
    [Fact]
    public void GeneratedWebhookContracts_ShouldExposeDataRecordForEveryKnownWebhook()
    {
        var dataTypes = typeof(TikTokOrderStatusChangeWebhookData).Assembly
            .GetTypes()
            .Where(type => type.Namespace == "TikTokPartnerSdk.Generated.Webhooks")
            .Where(type => type.IsClass && type.Name.EndsWith("WebhookData", StringComparison.Ordinal))
            .Select(type => type.Name)
            .ToArray();

        dataTypes.Should().HaveCount(39);
        dataTypes.Should().Contain("TikTokOrderStatusChangeWebhookData");
        dataTypes.Should().Contain("TikTokInventoryChangedWebhookData");
        dataTypes.Should().Contain("TikTokShoppableContentPostingWebhookData");
    }

    [Fact]
    public void GeneratedWebhookContracts_ShouldExposeTypedInventoryChangedFields()
    {
        var properties = typeof(TikTokInventoryChangedWebhookData)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        properties.Should().Contain([
            "EventId",
            "OccurredAt",
            "SellerId",
            "ProductId",
            "SkuId",
            "QuantitySnapshotAfterChange",
            "ChangeDetail"
        ]);
    }
}
