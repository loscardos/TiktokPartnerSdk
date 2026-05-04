using System.Net;
using TikTokPartnerSdk.Abstractions.Errors;

namespace TikTokPartnerSdk.Core.Http;

public static class TikTokErrorClassifier
{
    public static TikTokErrorCategory Classify(long code, string? message)
    {
        var text = message ?? string.Empty;
        if (text.Contains("token", StringComparison.OrdinalIgnoreCase))
        {
            return TikTokErrorCategory.Authentication;
        }

        if (text.Contains("sign", StringComparison.OrdinalIgnoreCase))
        {
            return TikTokErrorCategory.Signature;
        }

        if (text.Contains("rate", StringComparison.OrdinalIgnoreCase) || code == 429)
        {
            return TikTokErrorCategory.RateLimit;
        }

        if (code >= 500000)
        {
            return TikTokErrorCategory.Transient;
        }

        if (code != 0)
        {
            return TikTokErrorCategory.Validation;
        }

        return TikTokErrorCategory.Unknown;
    }

    public static TikTokErrorCategory Classify(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => TikTokErrorCategory.Authentication,
            HttpStatusCode.TooManyRequests => TikTokErrorCategory.RateLimit,
            >= HttpStatusCode.InternalServerError => TikTokErrorCategory.Transient,
            >= HttpStatusCode.BadRequest => TikTokErrorCategory.Validation,
            _ => TikTokErrorCategory.Unknown
        };
    }
}
