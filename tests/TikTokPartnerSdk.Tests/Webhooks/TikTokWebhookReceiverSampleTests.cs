using FluentAssertions;

namespace Loscardos.TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookReceiverSampleTests
{
    [Fact]
    public void WebhookReceiverSample_ShouldContainRunnableAspNetReceiver()
    {
        var root = TestPaths.RepositoryRoot;
        var projectPath = Path.Combine(root, "samples", "TikTokPartnerSdk.WebhookReceiver", "TikTokPartnerSdk.WebhookReceiver.csproj");
        var programPath = Path.Combine(root, "samples", "TikTokPartnerSdk.WebhookReceiver", "Program.cs");

        File.Exists(projectPath).Should().BeTrue();
        File.Exists(programPath).Should().BeTrue();

        var program = File.ReadAllText(programPath);
        program.Should().Contain("ITikTokWebhookParser");
        program.Should().Contain("TikTok-Signature");
        program.Should().Contain("x-tt-signature");
        program.Should().Contain("TIKTOK_SANDBOX_APP_KEY");
        program.Should().Contain("return Results.Ok");
        program.Should().Contain("InMemoryWebhookQueue");
    }
}
