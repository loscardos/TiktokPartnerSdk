namespace Loscardos.TikTokPartnerSdk.Abstractions.Auth;

public sealed record TikTokAuthorizationContext(
    TikTokAccessTokenKind AccessTokenKind,
    string AppKey,
    string? ShopCipher = null);
