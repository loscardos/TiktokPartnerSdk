using System.Text;
using System.Text.RegularExpressions;

namespace Loscardos.TikTokPartnerSdk.SampleConsole;

public enum EndpointClassification
{
    Readonly,
    Mutation
}

public enum EndpointCertificationStatus
{
    ReadonlyCertified,
    ReadonlyNeedsLiveCertification,
    DetailNeedsData,
    MutationNeedsFixture
}

public sealed record GeneratedEndpoint(
    string Area,
    string Method,
    string Path,
    string AccessTokenKind,
    bool RequiresShopCipher,
    bool RequiresPathId);

public sealed record EndpointCertificationRow(
    string Area,
    string Method,
    string Path,
    string AccessTokenKind,
    EndpointClassification Classification,
    EndpointCertificationStatus Status,
    bool RequiresShopCipher,
    bool RequiresPathId,
    bool RequiresFixture,
    string Detail);

public static partial class GeneratedEndpointInventory
{
    public static string DefaultGeneratedManagersDirectory => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory,
        "..",
        "..",
        "..",
        "..",
        "..",
        "src",
        "TikTokPartnerSdk.Core",
        "Managers",
        "Generated"));

    public static IReadOnlyList<GeneratedEndpoint> ReadFromDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Generated manager directory was not found: {directory}");
        }

        var endpoints = new List<GeneratedEndpoint>();
        foreach (var path in Directory.GetFiles(directory, "*Api.g.cs").OrderBy(static item => item, StringComparer.Ordinal))
        {
            endpoints.AddRange(ReadFromFile(path));
        }

        return endpoints
            .OrderBy(static endpoint => endpoint.Area, StringComparer.Ordinal)
            .ThenBy(static endpoint => endpoint.Path, StringComparer.Ordinal)
            .ThenBy(static endpoint => endpoint.Method, StringComparer.Ordinal)
            .ToArray();
    }

    private static IEnumerable<GeneratedEndpoint> ReadFromFile(string path)
    {
        var source = File.ReadAllText(path);
        var classMatch = ApiClassRegex().Match(source);
        if (!classMatch.Success)
        {
            yield break;
        }

        var area = classMatch.Groups["area"].Value;
        var methodMatches = ApiMethodRegex().Matches(source);
        for (var index = 0; index < methodMatches.Count; index++)
        {
            var start = methodMatches[index].Index;
            var end = index + 1 < methodMatches.Count ? methodMatches[index + 1].Index : source.Length;
            var block = source[start..end];
            var httpMethod = HttpMethodRegex().Match(block);
            var endpointPath = PathRegex().Match(block);
            if (!httpMethod.Success || !endpointPath.Success)
            {
                continue;
            }

            var accessTokenKind = AccessTokenKindRegex().Match(block);
            yield return new GeneratedEndpoint(
                area,
                httpMethod.Groups["method"].Value.ToUpperInvariant(),
                endpointPath.Groups["path"].Value,
                accessTokenKind.Success ? accessTokenKind.Groups["kind"].Value : string.Empty,
                block.Contains("\"shop_cipher\"", StringComparison.Ordinal),
                endpointPath.Groups["path"].Value.Contains('{', StringComparison.Ordinal));
        }
    }

    [GeneratedRegex(@"public sealed class (?<area>[A-Za-z0-9]+)Api\(")]
    private static partial Regex ApiClassRegex();

    [GeneratedRegex(@"public async Task<")]
    private static partial Regex ApiMethodRegex();

    [GeneratedRegex(@"HttpMethod\.(?<method>[A-Za-z]+)")]
    private static partial Regex HttpMethodRegex();

    [GeneratedRegex(@"var path = ""(?<path>[^""]+)"";")]
    private static partial Regex PathRegex();

    [GeneratedRegex(@"// access_token_kind=(?<kind>[^\r\n]+)")]
    private static partial Regex AccessTokenKindRegex();
}

public static class ReadonlyCertificationCatalog
{
    public static IReadOnlySet<string> CertifiedEndpoints { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        Key("GET", "/authorization/202309/shops"),
        Key("GET", "/seller/202309/shops"),
        Key("GET", "/seller/202309/permissions"),
        Key("GET", "/event/202309/webhooks"),
        Key("POST", "/order/202309/orders/search"),
        Key("POST", "/product/202502/products/search"),
        Key("GET", "/logistics/202309/warehouses"),
        Key("GET", "/logistics/202309/global_warehouses"),
        Key("POST", "/fulfillment/202309/packages/search"),
        Key("POST", "/return_refund/202602/cancellations/search"),
        Key("POST", "/return_refund/202602/returns/search"),
        Key("GET", "/finance/202309/payments"),
        Key("GET", "/finance/202309/statements"),
        Key("GET", "/finance/202309/withdrawals"),
        Key("GET", "/finance/202507/orders/unsettled")
    };

    public static string Key(string method, string path)
        => $"{method.ToUpperInvariant()} {path}";
}

public static class EndpointCertificationMatrix
{
    public static string DefaultPath => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory,
        "..",
        "..",
        "..",
        "..",
        "..",
        ".tmp",
        "tiktok-endpoint-certification-matrix.md"));

    public static IReadOnlyList<EndpointCertificationRow> Create(
        IReadOnlyList<GeneratedEndpoint> endpoints,
        IReadOnlySet<string> readonlyCertifiedEndpoints)
        => endpoints.Select(endpoint =>
        {
            var classification = Classify(endpoint);
            var status = ResolveStatus(endpoint, classification, readonlyCertifiedEndpoints);
            return new EndpointCertificationRow(
                endpoint.Area,
                endpoint.Method,
                endpoint.Path,
                endpoint.AccessTokenKind,
                classification,
                status,
                endpoint.RequiresShopCipher,
                endpoint.RequiresPathId,
                status is EndpointCertificationStatus.MutationNeedsFixture or EndpointCertificationStatus.DetailNeedsData,
                ResolveDetail(status));
        }).ToArray();

    public static void WriteMarkdown(string path, IReadOnlyList<EndpointCertificationRow> rows)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
        File.WriteAllText(path, ToMarkdown(rows));
    }

    public static string ToMarkdown(IReadOnlyList<EndpointCertificationRow> rows)
    {
        var lines = new List<string>
        {
            "# TikTok Partner SDK Endpoint Certification Matrix",
            string.Empty,
            $"Generated at: {DateTimeOffset.UtcNow:O}",
            string.Empty,
            $"Total endpoints: {rows.Count}",
            string.Empty,
            "| Status | Count |",
            "| --- | ---: |"
        };

        foreach (var group in rows.GroupBy(static row => row.Status).OrderBy(static group => group.Key.ToString()))
        {
            lines.Add($"| {group.Key} | {group.Count()} |");
        }

        lines.AddRange([
            string.Empty,
            "| Area | Method | Path | Auth | Class | Status | Shop cipher | Path ID | Fixture | Detail |",
            "| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |"
        ]);

        foreach (var row in rows.OrderBy(static row => row.Area, StringComparer.Ordinal).ThenBy(static row => row.Path, StringComparer.Ordinal).ThenBy(static row => row.Method, StringComparer.Ordinal))
        {
            lines.Add($"| {Escape(row.Area)} | {row.Method} | {Escape(row.Path)} | {Escape(row.AccessTokenKind)} | {row.Classification} | {row.Status} | {YesNo(row.RequiresShopCipher)} | {YesNo(row.RequiresPathId)} | {YesNo(row.RequiresFixture)} | {Escape(row.Detail)} |");
        }

        lines.Add(string.Empty);
        return string.Join(Environment.NewLine, lines);
    }

    private static EndpointClassification Classify(GeneratedEndpoint endpoint)
    {
        if (endpoint.Method == "GET")
        {
            return EndpointClassification.Readonly;
        }

        var normalized = endpoint.Path.ToLowerInvariant();
        return normalized.Contains("/search", StringComparison.Ordinal)
            || normalized.Contains("/get", StringComparison.Ordinal)
            || normalized.Contains("/query", StringComparison.Ordinal)
            || normalized.Contains("/check", StringComparison.Ordinal)
            || normalized.Contains("/calculate", StringComparison.Ordinal)
            || normalized.Contains("/recommend", StringComparison.Ordinal)
            || normalized.Contains("/download", StringComparison.Ordinal)
            || normalized.Contains("/precheck", StringComparison.Ordinal)
            ? EndpointClassification.Readonly
            : EndpointClassification.Mutation;
    }

    private static EndpointCertificationStatus ResolveStatus(
        GeneratedEndpoint endpoint,
        EndpointClassification classification,
        IReadOnlySet<string> readonlyCertifiedEndpoints)
    {
        if (readonlyCertifiedEndpoints.Contains(ReadonlyCertificationCatalog.Key(endpoint.Method, endpoint.Path)))
        {
            return EndpointCertificationStatus.ReadonlyCertified;
        }

        if (classification == EndpointClassification.Mutation)
        {
            return EndpointCertificationStatus.MutationNeedsFixture;
        }

        return endpoint.RequiresPathId
            ? EndpointCertificationStatus.DetailNeedsData
            : EndpointCertificationStatus.ReadonlyNeedsLiveCertification;
    }

    private static string ResolveDetail(EndpointCertificationStatus status)
        => status switch
        {
            EndpointCertificationStatus.ReadonlyCertified => "live readonly certification passed",
            EndpointCertificationStatus.ReadonlyNeedsLiveCertification => "readonly endpoint needs live certification coverage",
            EndpointCertificationStatus.DetailNeedsData => "detail endpoint needs sandbox data or explicit ID fixture",
            EndpointCertificationStatus.MutationNeedsFixture => "write endpoint needs opt-in sandbox fixture",
            _ => string.Empty
        };

    private static string YesNo(bool value) => value ? "yes" : "no";

    private static string Escape(string value)
        => value.Replace("|", "\\|", StringComparison.Ordinal).ReplaceLineEndings(" ");
}
