using System.Text;

namespace Loscardos.TikTokPartnerSdk.Generator;

public sealed class EndpointCoverageWriter
{
    public string WriteMarkdown(IReadOnlyList<SchemaEndpoint> endpoints)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Endpoint Coverage");
        builder.AppendLine();
        builder.AppendLine($"Total schema endpoints: {endpoints.Count}");

        foreach (var group in endpoints.GroupBy(static endpoint => endpoint.ModuleKey).OrderBy(static group => group.Key, StringComparer.Ordinal))
        {
            builder.AppendLine();
            builder.AppendLine($"## {group.Key}");

            foreach (var endpoint in group.OrderBy(static item => item.Path, StringComparer.Ordinal))
            {
                builder.AppendLine($"- `{endpoint.Method}` `{endpoint.Path}`");
                builder.AppendLine($"  - access_token_kind={endpoint.AccessTokenKind}");
                builder.AppendLine($"  - required_headers={string.Join(",", endpoint.RequiredHeaders)}");
                builder.AppendLine($"  - request_parameters={endpoint.RequestParameters.Count}");
                builder.AppendLine($"  - response_parameters={endpoint.ResponseParameters.Count}");
            }
        }

        return builder.ToString();
    }
}
