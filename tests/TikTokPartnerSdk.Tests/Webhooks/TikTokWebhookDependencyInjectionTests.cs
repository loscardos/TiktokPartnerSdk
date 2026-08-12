using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;
using Loscardos.TikTokPartnerSdk.Core.Webhooks;
using Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection;

namespace Loscardos.TikTokPartnerSdk.Tests.Webhooks;

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
