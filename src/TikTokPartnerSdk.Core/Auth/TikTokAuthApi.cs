using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.Http;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;

namespace Loscardos.TikTokPartnerSdk.Core.Auth;

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
        var envelope = await authClient.GetAsync<AuthTokenPayload>(
            "/token/get",
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

        var token = await RefreshTokenAsync(
            context,
            existing,
            cancellationToken);
        await tokenStore.StoreAsync(token, cancellationToken);
        return token;
    }

    public async Task<TikTokTokenRecord> RefreshTokenAsync(
        TikTokAuthorizationContext context,
        TikTokTokenRecord existingToken,
        CancellationToken cancellationToken)
    {
        if (existingToken.AccessTokenKind != context.AccessTokenKind
            || !string.Equals(
                existingToken.AppKey,
                context.AppKey,
                StringComparison.Ordinal)
            || !string.Equals(
                existingToken.ShopCipher,
                context.ShopCipher,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "The explicit TikTok token does not match the authorization context.",
                nameof(existingToken));
        }

        var envelope = await authClient.GetAsync<AuthTokenPayload>(
            "/token/refresh",
            new Dictionary<string, object?>
            {
                ["app_key"] = _options.AppKey,
                ["app_secret"] = _options.AppSecret,
                ["refresh_token"] = existingToken.RefreshToken,
                ["grant_type"] = "refresh_token"
            },
            cancellationToken);

        var payload = envelope.Data ?? throw new InvalidOperationException("TikTok auth refresh returned no data.");
        return ToTokenRecord(context, payload);
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
            ToExpiry(payload.AccessTokenExpireIn, now),
            ToExpiry(payload.RefreshTokenExpireIn, now),
            context.ShopCipher,
            context.AppKey);
    }

    private static DateTimeOffset ToExpiry(long value, DateTimeOffset now)
    {
        var nowUnix = now.ToUnixTimeSeconds();
        return value > nowUnix
            ? DateTimeOffset.FromUnixTimeSeconds(value)
            : now.AddSeconds(value);
    }

    private sealed record AuthTokenPayload(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken,
        [property: JsonPropertyName("access_token_expire_in")] long AccessTokenExpireIn,
        [property: JsonPropertyName("refresh_token_expire_in")] long RefreshTokenExpireIn);
}
