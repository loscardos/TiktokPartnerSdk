namespace TikTokPartnerSdk.Abstractions.Auth;

public interface ITikTokTokenStore
{
    Task<TikTokTokenRecord?> GetAsync(
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken);

    Task StoreAsync(
        TikTokTokenRecord token,
        CancellationToken cancellationToken);

    Task ClearAsync(
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken);
}
