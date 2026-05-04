using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Extensions.DependencyInjection;

namespace TikTokPartnerSdk.IntegrationTests;

internal static class TikTokSandboxTestSupport
{
    public static bool HasSellerAuthorization(TikTokSandboxConfiguration? configuration)
        => configuration is not null
           && !string.IsNullOrWhiteSpace(configuration.AuthCode)
           && !string.IsNullOrWhiteSpace(configuration.ShopCipher);

    public static ServiceProvider CreateProvider(TikTokSandboxConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddTikTokPartnerSdk(options =>
        {
            options.AppKey = configuration.AppKey;
            options.AppSecret = configuration.AppSecret;
        });

        return services.BuildServiceProvider();
    }

    public static async Task<TikTokTokenRecord?> TryExchangeSellerTokenAsync(
        IServiceProvider provider,
        TikTokSandboxConfiguration? configuration,
        CancellationToken cancellationToken)
    {
        if (!HasSellerAuthorization(configuration))
        {
            return null;
        }

        var authApi = provider.GetRequiredService<IAuthApi>();
        return await authApi.ExchangeCodeAsync(
            configuration!.AuthCode,
            new TikTokAuthorizationContext(
                TikTokAccessTokenKind.Seller,
                configuration.AppKey,
                configuration.ShopCipher),
            cancellationToken);
    }
}
