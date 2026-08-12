using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;

namespace Loscardos.TikTokPartnerSdk.Core.Auth;

public sealed class TikTokTokenService(
    ITikTokTokenStore tokenStore,
    IAuthApi authApi,
    IOptions<TikTokPartnerOptions> options)
{
    private readonly TikTokPartnerOptions _options = options.Value;

    public async Task<TikTokTokenRecord> GetValidTokenAsync(
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        var token = await tokenStore.GetAsync(context, cancellationToken)
            ?? throw new InvalidOperationException("TikTok token is missing. Authorize first before calling authenticated endpoints.");

        if (!NeedsRefresh(token))
        {
            return token;
        }

        var refreshed = await authApi.RefreshTokenAsync(context, cancellationToken);
        await tokenStore.StoreAsync(refreshed, cancellationToken);
        return refreshed;
    }

    private bool NeedsRefresh(TikTokTokenRecord token)
        => token.ExpiresAtUtc <= DateTimeOffset.UtcNow.Add(_options.TokenRefreshSkew);
}
