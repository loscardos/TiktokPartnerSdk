using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Core.Auth;
using TikTokPartnerSdk.Core.Http;

namespace TikTokPartnerSdk.Extensions.DependencyInjection;

public static class TikTokServiceCollectionExtensions
{
    public static IServiceCollection AddTikTokPartnerSdk(
        this IServiceCollection services,
        Action<TikTokPartnerOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<TikTokRequestUriBuilder>();
        services.AddSingleton<TikTokRequestContentFactory>();
        services.AddSingleton<TikTokResponseParser>();
        services.AddHttpClient<ITikTokPartnerClient, TikTokPartnerClient>();
        services.AddSingleton<ITikTokTokenStore, InMemoryTikTokTokenStore>();
        services.AddScoped<TikTokTokenService>();
        services.AddScoped<IAuthApi, TikTokAuthApi>();
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
