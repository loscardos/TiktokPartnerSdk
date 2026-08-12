using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Loscardos.TikTokPartnerSdk.Abstractions.Http;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated;
using Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection;

namespace Loscardos.TikTokPartnerSdk.Tests.DependencyInjection;

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
    public void AddTikTokPartnerSdk_should_register_polished_managers()
    {
        var services = new ServiceCollection();
        services.AddTikTokPartnerSdk(options =>
        {
            options.AppKey = "key";
            options.AppSecret = "secret";
        });

        var provider = services.BuildServiceProvider();

        provider.GetService<IOrderManager>().Should().NotBeNull();
        provider.GetService<IProductManager>().Should().NotBeNull();
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
        provider.GetService<IPromotionApi>().Should().NotBeNull();
        provider.GetService<IAnalyticsApi>().Should().NotBeNull();
        provider.GetService<ICustomerServiceApi>().Should().NotBeNull();
        provider.GetService<ICustomerEngagementApi>().Should().NotBeNull();
        provider.GetService<IAffiliateCreatorApi>().Should().NotBeNull();
        provider.GetService<IAffiliatePartnerApi>().Should().NotBeNull();
        provider.GetService<IAffiliateSellerApi>().Should().NotBeNull();
        provider.GetService<IToolsApi>().Should().NotBeNull();
    }
}
