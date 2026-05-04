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

        var parsed = ParseTikTokSignatureHeader(signature);
        var normalizedSignature = parsed.Signature ?? NormalizeSignature(signature);
        var signedPayload = parsed.Timestamp is null
            ? rawBody
            : $"{parsed.Timestamp.Value}.{rawBody}";
        var expected = ComputeHmac(signedPayload);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(normalizedSignature));
    }

    public long? GetSignedTimestamp(string signature)
        => ParseTikTokSignatureHeader(signature).Timestamp;

    private static string NormalizeSignature(string signature)
    {
        var trimmed = signature.Trim();
        const string sha256Prefix = "sha256=";
        return trimmed.StartsWith(sha256Prefix, StringComparison.OrdinalIgnoreCase)
            ? trimmed[sha256Prefix.Length..].ToLowerInvariant()
            : trimmed.ToLowerInvariant();
    }

    private string ComputeHmac(string payload)
        => Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(_options.AppSecret),
            Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();

    private static ParsedTikTokSignatureHeader ParseTikTokSignatureHeader(string signature)
    {
        long? timestamp = null;
        string? signedSignature = null;

        foreach (var part in signature.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var separator = part.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            var key = part[..separator];
            var value = part[(separator + 1)..];
            if (string.Equals(key, "t", StringComparison.OrdinalIgnoreCase)
                && long.TryParse(value, out var parsedTimestamp))
            {
                timestamp = parsedTimestamp;
            }
            else if (string.Equals(key, "s", StringComparison.OrdinalIgnoreCase))
            {
                signedSignature = NormalizeSignature(value);
            }
        }

        return new ParsedTikTokSignatureHeader(timestamp, signedSignature);
    }

    private sealed record ParsedTikTokSignatureHeader(long? Timestamp, string? Signature);
}
