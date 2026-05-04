using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Abstractions.RateLimiting;
using TikTokPartnerSdk.Core.Auth;
using TikTokPartnerSdk.Core.Crypto;
using TikTokPartnerSdk.Core.Http;
using TikTokPartnerSdk.Core.Managers.Generated;
using TikTokPartnerSdk.Core.RateLimiting;

namespace TikTokPartnerSdk.Extensions.DependencyInjection;

public static class TikTokServiceCollectionExtensions
{
    public static IServiceCollection AddTikTokPartnerSdk(
        this IServiceCollection services,
        Action<TikTokPartnerOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<TikTokRequestSigner>();
        services.AddSingleton<TikTokRequestUriBuilder>();
        services.AddSingleton<TikTokRequestContentFactory>();
        services.AddSingleton<TikTokResponseParser>();
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
