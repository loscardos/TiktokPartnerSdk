using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Http;
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
        provider.GetService<ITikTokAuthClient>().Should().NotBeNull();
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
        provider.GetService<IOrderApi>().Should().NotBeNull();
        provider.GetService<IProductApi>().Should().NotBeNull();
        provider.GetService<IFulfillmentApi>().Should().NotBeNull();
        provider.GetService<ILogisticsApi>().Should().NotBeNull();
        provider.GetService<IReturnAndRefundApi>().Should().NotBeNull();
        provider.GetService<IFinanceApi>().Should().NotBeNull();
        provider.GetService<IFulfilledByTiktokFbtApi>().Should().NotBeNull();
        provider.GetService<ISupplyChainApi>().Should().NotBeNull();
    }
}
