using System.Security.Cryptography;
using System.Text;

namespace Loscardos.TikTokPartnerSdk.Core.Crypto;

public sealed class TikTokRequestSigner
{
    public string Sign(
        string appSecret,
        string path,
        IReadOnlyDictionary<string, string?> query,
        string? body)
    {
        var canonicalQuery = string.Join(
            string.Empty,
            query
                .Where(static x =>
                    !string.Equals(x.Key, "sign", StringComparison.Ordinal)
                    && !string.Equals(x.Key, "access_token", StringComparison.Ordinal))
                .OrderBy(x => x.Key, StringComparer.Ordinal)
                .Select(x => $"{x.Key}{x.Value}"));
        var payload = $"{appSecret}{path}{canonicalQuery}{body ?? string.Empty}{appSecret}";
        var keyBytes = Encoding.UTF8.GetBytes(appSecret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var hash = HMACSHA256.HashData(keyBytes, payloadBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
