using Loscardos.TikTokPartnerSdk.Abstractions.Auth;

namespace Loscardos.TikTokPartnerSdk.Storage.EntityFramework;

public sealed class TikTokTokenEntity
{
    public int Id { get; set; }

    public TikTokAccessTokenKind AccessTokenKind { get; set; }

    public string AppKey { get; set; } = string.Empty;

    public string? ShopCipher { get; set; }

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; set; }

    public DateTimeOffset RefreshTokenExpiresAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }
}
