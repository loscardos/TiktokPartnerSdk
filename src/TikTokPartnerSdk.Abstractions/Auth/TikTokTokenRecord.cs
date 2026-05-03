namespace TikTokPartnerSdk.Abstractions.Auth;

public sealed record TikTokTokenRecord(
    TikTokAccessTokenKind AccessTokenKind,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    string? ShopCipher,
    string AppKey);
