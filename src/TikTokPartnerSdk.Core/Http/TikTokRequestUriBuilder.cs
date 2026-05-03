using System.Globalization;
using TikTokPartnerSdk.Abstractions.Configuration;

namespace TikTokPartnerSdk.Core.Http;

public sealed class TikTokRequestUriBuilder
{
    public Uri Build(
        TikTokPartnerOptions options,
        string path,
        IReadOnlyDictionary<string, string?> query)
    {
        var baseUri = options.OpenApiBaseUrl.TrimEnd('/');
        var normalizedPath = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
        var uri = baseUri + normalizedPath;

        if (query.Count == 0)
        {
            return new Uri(uri);
        }

        var queryString = string.Join(
            "&",
            query.Select(static x =>
                $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value ?? string.Empty)}"));

        return new Uri(
            string.Create(
                CultureInfo.InvariantCulture,
                $"{uri}?{queryString}"));
    }
}
