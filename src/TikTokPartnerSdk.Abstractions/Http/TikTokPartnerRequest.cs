using Loscardos.TikTokPartnerSdk.Abstractions.Auth;

namespace Loscardos.TikTokPartnerSdk.Abstractions.Http;

public sealed record TikTokPartnerRequest(
    HttpMethod Method,
    string Path,
    IReadOnlyDictionary<string, object?> Query,
    object? Body,
    TikTokAuthorizationContext? Authorization,
    string? AccessToken = null);
