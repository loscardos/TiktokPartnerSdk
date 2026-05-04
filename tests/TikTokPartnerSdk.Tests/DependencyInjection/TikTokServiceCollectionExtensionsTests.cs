using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Extensions.DependencyInjection;

namespace TikTokPartnerSdk.Tests.DependencyInjection;

public sealed class TikTokServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTikTokPartnerSdk_should_register_auth_api()
    {
        var services = new ServiceCollection();
        services.AddTikTokPartnerSdk(options =>
        {
            options.AppKey = "key";
            options.AppSecret = "secret";
        });

        var provider = services.BuildServiceProvider();

        provider.GetService<IAuthApi>().Should().NotBeNull();
    }

    [Fact]
    public void AddTikTokPartnerSdk_should_register_generated_manager_apis()
    {
        var services = new ServiceCollection();
        services.AddTikTokPartnerSdk(options =>
        {
            options.AppKey = "key";
            options.AppSecret = "secret";
        });

        var provider = services.BuildServiceProvider();

        provider.GetService<IAuthorizationApi>().Should().NotBeNull();
        provider.GetService<ISellerApi>().Should().NotBeNull();
        provider.GetService<IEventApi>().Should().NotBeNull();
    }
}
