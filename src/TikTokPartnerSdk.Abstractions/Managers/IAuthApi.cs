using TikTokPartnerSdk.Abstractions.Auth;

namespace TikTokPartnerSdk.Abstractions.Managers;

public interface IAuthApi
{
    Uri BuildAuthorizationUrl(Uri redirectUri, string? state);

    Task<TikTokTokenRecord> ExchangeCodeAsync(
        string code,
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken);

    Task<TikTokTokenRecord> RefreshTokenAsync(
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken);

    Task<TikTokTokenRecord> RefreshTokenAsync(
        TikTokAuthorizationContext context,
        TikTokTokenRecord existingToken,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException(
            "Explicit caller-owned token refresh is not implemented.");
}
