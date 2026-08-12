using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.Http;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated;
using Loscardos.TikTokPartnerSdk.Abstractions.RateLimiting;
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;
using Loscardos.TikTokPartnerSdk.Core.Auth;
using Loscardos.TikTokPartnerSdk.Core.Crypto;
using Loscardos.TikTokPartnerSdk.Core.Http;
using Loscardos.TikTokPartnerSdk.Core.Managers;
using Loscardos.TikTokPartnerSdk.Core.Managers.Generated;
using Loscardos.TikTokPartnerSdk.Core.RateLimiting;
using Loscardos.TikTokPartnerSdk.Core.Webhooks;

namespace Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection;

public static class TikTokServiceCollectionExtensions
{
    public static IServiceCollection AddTikTokPartnerSdk(
        this IServiceCollection services,
        Action<TikTokPartnerOptions> configure)
    {
        services.Configure(configure);
        services.Configure<TikTokWebhookOptions>(_ => { });
        services.AddSingleton<TikTokRequestSigner>();
        services.AddSingleton<TikTokRequestUriBuilder>();
        services.AddSingleton<TikTokRequestContentFactory>();
        services.AddSingleton<TikTokResponseParser>();
        services.AddSingleton<ITikTokWebhookSignatureVerifier, TikTokWebhookSignatureVerifier>();
        services.AddSingleton<TikTokWebhookTimestampValidator>();
        services.AddSingleton<ITikTokWebhookIdempotencyKeyFactory, TikTokWebhookIdempotencyKeyFactory>();
        services.AddSingleton<ITikTokWebhookParser, TikTokWebhookParser>();
        services.AddSingleton<NoopTikTokRateLimiter>();
        services.AddSingleton<FixedWindowTikTokRateLimiter>();
        services.AddSingleton<ITikTokRateLimiter>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<TikTokPartnerOptions>>().Value;
            return options.EnableRateLimiting
                ? provider.GetRequiredService<FixedWindowTikTokRateLimiter>()
                : provider.GetRequiredService<NoopTikTokRateLimiter>();
        });
        services.AddHttpClient<ITikTokAuthClient, TikTokAuthClient>();
        services.AddHttpClient<ITikTokPartnerClient, TikTokPartnerClient>();
        services.AddSingleton<ITikTokTokenStore, InMemoryTikTokTokenStore>();
        services.AddScoped<TikTokTokenService>();
        services.AddScoped<IAuthApi, TikTokAuthApi>();
        services.AddScoped<IOrderManager, OrderManager>();
        services.AddScoped<IProductManager, ProductManager>();
        services.AddScoped<IAuthorizationApi, AuthorizationApi>();
        services.AddScoped<ISellerApi, SellerApi>();
        services.AddScoped<IEventApi, EventApi>();
        services.AddScoped<IOrderApi, OrderApi>();
        services.AddScoped<IProductApi, ProductApi>();
        services.AddScoped<IFulfillmentApi, FulfillmentApi>();
        services.AddScoped<ILogisticsApi, LogisticsApi>();
        services.AddScoped<IReturnAndRefundApi, ReturnAndRefundApi>();
        services.AddScoped<IFinanceApi, FinanceApi>();
        services.AddScoped<IFulfilledByTiktokFbtApi, FulfilledByTiktokFbtApi>();
        services.AddScoped<ISupplyChainApi, SupplyChainApi>();
        services.AddScoped<IPromotionApi, PromotionApi>();
        services.AddScoped<IAnalyticsApi, AnalyticsApi>();
        services.AddScoped<ICustomerServiceApi, CustomerServiceApi>();
        services.AddScoped<ICustomerEngagementApi, CustomerEngagementApi>();
        services.AddScoped<IAffiliateCreatorApi, AffiliateCreatorApi>();
        services.AddScoped<IAffiliatePartnerApi, AffiliatePartnerApi>();
        services.AddScoped<IAffiliateSellerApi, AffiliateSellerApi>();
        services.AddScoped<IToolsApi, ToolsApi>();
        return services;
    }

    private sealed class InMemoryTikTokTokenStore : ITikTokTokenStore
    {
        private readonly Dictionary<string, TikTokTokenRecord> _tokens = new(StringComparer.Ordinal);

        public Task<TikTokTokenRecord?> GetAsync(
            TikTokAuthorizationContext context,
            CancellationToken cancellationToken)
        {
            _tokens.TryGetValue(ToKey(context), out var token);
            return Task.FromResult(token);
        }

        public Task StoreAsync(TikTokTokenRecord token, CancellationToken cancellationToken)
        {
            _tokens[ToKey(new TikTokAuthorizationContext(token.AccessTokenKind, token.AppKey, token.ShopCipher))] = token;
            return Task.CompletedTask;
        }

        public Task ClearAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
        {
            _tokens.Remove(ToKey(context));
            return Task.CompletedTask;
        }

        private static string ToKey(TikTokAuthorizationContext context)
            => $"{context.AccessTokenKind}:{context.AppKey}:{context.ShopCipher ?? string.Empty}";
    }
}
