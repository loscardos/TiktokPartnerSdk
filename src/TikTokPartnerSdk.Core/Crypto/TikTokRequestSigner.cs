using System.Security.Cryptography;
using System.Text;

namespace TikTokPartnerSdk.Core.Crypto;

public sealed class TikTokRequestSigner
{
    public string Sign(
        string appSecret,
        string path,
        IReadOnlyDictionary<string, string?> query,
        string? body)
    {
        var canonicalQuery = string.Join(
            "&",
            query
                .OrderBy(x => x.Key, StringComparer.Ordinal)
                .Select(x => $"{x.Key}{x.Value}"));
        var payload = $"{path}{canonicalQuery}{body ?? string.Empty}";
        var keyBytes = Encoding.UTF8.GetBytes(appSecret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var hash = HMACSHA256.HashData(keyBytes, payloadBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
