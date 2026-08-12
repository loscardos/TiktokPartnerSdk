using Microsoft.EntityFrameworkCore;
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;

namespace Loscardos.TikTokPartnerSdk.Storage.EntityFramework;

public sealed class EfTikTokTokenStore(
    TikTokTokenDbContext dbContext,
    ITikTokTokenProtector tokenProtector) : ITikTokTokenStore
{
    public async Task<TikTokTokenRecord?> GetAsync(
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.TikTokTokens
            .SingleOrDefaultAsync(
                token => token.AccessTokenKind == context.AccessTokenKind
                    && token.AppKey == context.AppKey
                    && token.ShopCipher == context.ShopCipher,
                cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return new TikTokTokenRecord(
            entity.AccessTokenKind,
            tokenProtector.Unprotect(entity.AccessToken),
            tokenProtector.Unprotect(entity.RefreshToken),
            entity.ExpiresAtUtc,
            entity.RefreshTokenExpiresAtUtc,
            entity.ShopCipher,
            entity.AppKey);
    }

    public async Task StoreAsync(
        TikTokTokenRecord token,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.TikTokTokens
            .SingleOrDefaultAsync(
                item => item.AccessTokenKind == token.AccessTokenKind
                    && item.AppKey == token.AppKey
                    && item.ShopCipher == token.ShopCipher,
                cancellationToken);

        if (entity is null)
        {
            entity = new TikTokTokenEntity
            {
                AccessTokenKind = token.AccessTokenKind,
                AppKey = token.AppKey,
                ShopCipher = token.ShopCipher
            };
            dbContext.TikTokTokens.Add(entity);
        }

        entity.AccessToken = tokenProtector.Protect(token.AccessToken);
        entity.RefreshToken = tokenProtector.Protect(token.RefreshToken);
        entity.ExpiresAtUtc = token.ExpiresAtUtc;
        entity.RefreshTokenExpiresAtUtc = token.RefreshTokenExpiresAtUtc;
        entity.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearAsync(
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.TikTokTokens
            .SingleOrDefaultAsync(
                token => token.AccessTokenKind == context.AccessTokenKind
                    && token.AppKey == context.AppKey
                    && token.ShopCipher == context.ShopCipher,
                cancellationToken);

        if (entity is null)
        {
            return;
        }

        dbContext.TikTokTokens.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
