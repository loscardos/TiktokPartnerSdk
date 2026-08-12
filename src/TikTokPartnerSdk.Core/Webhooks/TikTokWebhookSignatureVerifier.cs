using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;

namespace Loscardos.TikTokPartnerSdk.Core.Webhooks;

public sealed class TikTokWebhookSignatureVerifier(IOptions<TikTokPartnerOptions> options)
    : ITikTokWebhookSignatureVerifier
{
    private readonly TikTokPartnerOptions _options = options.Value;

    public bool Verify(string rawBody, string signature)
        => Verify(string.Empty, rawBody, signature);

    public bool Verify(string path, string rawBody, string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        var parsed = ParseTikTokSignatureHeader(signature);
        var normalizedSignature = parsed.Signature ?? NormalizeSignature(signature);
        var secret = GetSecret();
        var expectedSignatures = GetExpectedSignatures(path, rawBody, parsed.Timestamp, secret, _options.AppKey);

        return expectedSignatures.Any(expected => CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(normalizedSignature)));
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

    private string GetSecret()
        => string.IsNullOrWhiteSpace(_options.WebhookSecret)
            ? _options.AppSecret
            : _options.WebhookSecret;

    private static IEnumerable<string> GetExpectedSignatures(
        string path,
        string rawBody,
        long? signedTimestamp,
        string secret,
        string appKey)
    {
        if (!string.IsNullOrWhiteSpace(appKey))
        {
            yield return ComputeRawHmac($"{appKey}{rawBody}", secret);
        }

        if (!string.IsNullOrWhiteSpace(path))
        {
            yield return ComputeWrappedHmac($"{path}{rawBody}", secret);
        }

        if (signedTimestamp is not null)
        {
            yield return ComputeRawHmac($"{signedTimestamp.Value}.{rawBody}", secret);
        }

        yield return ComputeRawHmac(rawBody, secret);
    }

    private static string ComputeWrappedHmac(string payload, string secret)
        => Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(secret),
            Encoding.UTF8.GetBytes($"{secret}{payload}{secret}"))).ToLowerInvariant();

    private static string ComputeRawHmac(string payload, string secret)
        => Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(secret),
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
