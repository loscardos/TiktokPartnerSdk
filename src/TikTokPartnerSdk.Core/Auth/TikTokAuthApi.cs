using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.Managers;

namespace TikTokPartnerSdk.Core.Auth;

public sealed class TikTokAuthApi(
    IOptions<TikTokPartnerOptions> options,
    ITikTokPartnerClient client,
    ITikTokTokenStore tokenStore) : IAuthApi
{
    private readonly TikTokPartnerOptions _options = options.Value;
    private readonly ITikTokPartnerClient _client = client;

    public Uri BuildAuthorizationUrl(Uri redirectUri, string? state)
    {
        var builder = new UriBuilder("https://services.tiktokshop.com/open/authorize")
        {
            Query =
                $"app_key={Uri.EscapeDataString(_options.AppKey)}&redirect_uri={Uri.EscapeDataString(redirectUri.ToString())}&state={Uri.EscapeDataString(state ?? string.Empty)}"
        };
        return builder.Uri;
    }

    public async Task<TikTokTokenRecord> RefreshTokenAsync(
        TikTokAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        _ = _client;
        var existing = await tokenStore.GetAsync(context, cancellationToken)
            ?? throw new InvalidOperationException("TikTok token is missing for the requested authorization context.");

        return existing;
    }
}
