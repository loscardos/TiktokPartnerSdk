using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookSpecSnapshotTests
{
    [Fact]
    public void CommittedWebhookSnapshot_ShouldContainAllExtractedWebhookSpecs()
    {
        var path = Path.Combine(TestPaths.RepositoryRoot, "docs", "webhooks", "tiktok-webhooks.yaml");

        File.Exists(path).Should().BeTrue();
        var text = File.ReadAllText(path);

        text.Should().Contain("count: 39");
        text.Should().Contain("name: \"Order status change\"");
        text.Should().Contain("name: \"Product status change\"");
        text.Should().Contain("name: \"Seller deauthorization\"");
        text.Should().Contain("name: \"Shoppable content posting\"");
        text.Should().Contain("fields:");
        text.Should().Contain("example: |");
    }

    [Fact]
    public void IgnoredTempWebhookSnapshot_ShouldNotBeRequiredByTests()
    {
        var committedPath = Path.Combine(TestPaths.RepositoryRoot, "docs", "webhooks", "tiktok-webhooks.yaml");

        File.Exists(committedPath).Should().BeTrue();
    }
}
