using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Webhooks;
using TikTokPartnerSdk.Core.Webhooks;
using TikTokPartnerSdk.Extensions.DependencyInjection;

namespace TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookDependencyInjectionTests
{
    [Fact]
    public void AddTikTokPartnerSdk_RegistersWebhookServices()
    {
        var services = new ServiceCollection();

        services.AddTikTokPartnerSdk(options =>
        {
            options.AppKey = "app-key";
            options.AppSecret = "app-secret";
            options.OpenApiBaseUrl = "https://open-api.tiktokglobalshop.com";
        });

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ITikTokWebhookSignatureVerifier>().Should().NotBeNull();
        provider.GetRequiredService<ITikTokWebhookIdempotencyKeyFactory>().Should().NotBeNull();
        provider.GetRequiredService<ITikTokWebhookParser>().Should().NotBeNull();
        provider.GetRequiredService<TikTokWebhookTimestampValidator>().Should().NotBeNull();
    }
}
