using TikTokPartnerSdk.Abstractions.Auth;

namespace TikTokPartnerSdk.Core.Managers;

internal static class TikTokManagerContext
{
    public static string RequireShopCipher(TikTokAuthorizationContext context)
        => string.IsNullOrWhiteSpace(context.ShopCipher)
            ? throw new InvalidOperationException("TikTok shop cipher is required for seller scoped manager calls.")
            : context.ShopCipher;
}
