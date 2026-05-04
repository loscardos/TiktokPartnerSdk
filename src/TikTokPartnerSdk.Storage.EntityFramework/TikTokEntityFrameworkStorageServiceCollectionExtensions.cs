using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Auth;

namespace TikTokPartnerSdk.Storage.EntityFramework;

public static class TikTokEntityFrameworkStorageServiceCollectionExtensions
{
    public static IServiceCollection AddTikTokEntityFrameworkTokenStorage(this IServiceCollection services)
    {
        services.AddSingleton<ITikTokTokenProtector, DataProtectionTikTokTokenProtector>();
        services.AddScoped<ITikTokTokenStore, EfTikTokTokenStore>();
        return services;
    }
}
