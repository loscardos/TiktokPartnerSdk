using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Webhooks;

namespace TikTokPartnerSdk.Core.Webhooks;

public sealed class TikTokWebhookSignatureVerifier(IOptions<TikTokPartnerOptions> options)
    : ITikTokWebhookSignatureVerifier
{
    private readonly TikTokPartnerOptions _options = options.Value;

    public bool Verify(string rawBody, string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        var normalizedSignature = NormalizeSignature(signature);
        var expected = Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(_options.AppSecret),
            Encoding.UTF8.GetBytes(rawBody))).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(normalizedSignature));
    }

    private static string NormalizeSignature(string signature)
    {
        var trimmed = signature.Trim();
        const string sha256Prefix = "sha256=";
        return trimmed.StartsWith(sha256Prefix, StringComparison.OrdinalIgnoreCase)
            ? trimmed[sha256Prefix.Length..].ToLowerInvariant()
            : trimmed.ToLowerInvariant();
    }
}
