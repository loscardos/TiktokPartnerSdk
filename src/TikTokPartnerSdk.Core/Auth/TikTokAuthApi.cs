using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.Managers;

namespace TikTokPartnerSdk.Core.Auth;

public sealed class TikTokAuthApi(
    IOptions<TikTokPartnerOptions> options,
    ITikTokAuthClient authClient,
    ITikTokTokenStore tokenStore) : IAuthApi
{
    private readonly TikTokPartnerOptions _options = options.Value;

    public Uri BuildAuthorizationUrl(Uri redirectUri, string? state)
    {
        var builder = new UriBuilder("https://services.tiktokshop.com/open/authorize")
        {
            Query =
                $"app_key={Uri.EscapeDataString(_options.AppKey)}&redirect_uri={Uri.EscapeDataString(redirectUri.ToString())}&state={Uri.EscapeDataString(state ?? string.Empty)}"
        };
        return builder.Uri;
    }

    public async Task<TikTokTokenRecord> ExchangeCodeAsync(
        string code,
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        var envelope = await authClient.PostAsync<AuthTokenPayload>(
            "/authorization/202309/access_token",
            new Dictionary<string, object?>
            {
                ["app_key"] = _options.AppKey,
                ["app_secret"] = _options.AppSecret,
                ["auth_code"] = code,
                ["grant_type"] = "authorized_code"
            },
            cancellationToken);

        var payload = envelope.Data ?? throw new InvalidOperationException("TikTok auth exchange returned no data.");
        var token = ToTokenRecord(context, payload);
        await tokenStore.StoreAsync(token, cancellationToken);
        return token;
    }

    public async Task<TikTokTokenRecord> RefreshTokenAsync(
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        var existing = await tokenStore.GetAsync(context, cancellationToken)
            ?? throw new InvalidOperationException("TikTok token is missing for the requested authorization context.");

        var envelope = await authClient.PostAsync<AuthTokenPayload>(
            "/authorization/202309/refresh_token",
            new Dictionary<string, object?>
            {
                ["app_key"] = _options.AppKey,
                ["app_secret"] = _options.AppSecret,
                ["refresh_token"] = existing.RefreshToken,
                ["grant_type"] = "refresh_token"
            },
            cancellationToken);

        var payload = envelope.Data ?? throw new InvalidOperationException("TikTok auth refresh returned no data.");
        var token = ToTokenRecord(context, payload);
        await tokenStore.StoreAsync(token, cancellationToken);
        return token;
    }

    private static TikTokTokenRecord ToTokenRecord(
        TikTokAuthorizationContext context,
        AuthTokenPayload payload)
    {
        var now = DateTimeOffset.UtcNow;
        return new TikTokTokenRecord(
            context.AccessTokenKind,
            payload.AccessToken,
            payload.RefreshToken,
            now.AddSeconds(payload.AccessTokenExpireIn),
            now.AddSeconds(payload.RefreshTokenExpireIn),
            context.ShopCipher,
            context.AppKey);
    }

    private sealed record AuthTokenPayload(
        string AccessToken,
        string RefreshToken,
        long AccessTokenExpireIn,
        long RefreshTokenExpireIn);
}
