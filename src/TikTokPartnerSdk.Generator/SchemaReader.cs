using System.Text.Json;

namespace Loscardos.TikTokPartnerSdk.Generator;

public sealed class SchemaReader
{
    public IReadOnlyList<SchemaEndpoint> ReadDirectory(string schemaDirectory)
    {
        var endpoints = new List<SchemaEndpoint>();

        foreach (var file in Directory.EnumerateFiles(schemaDirectory, "*.json").OrderBy(path => path, StringComparer.Ordinal))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(file));
            var root = document.RootElement;

            endpoints.Add(new SchemaEndpoint(
                root.GetProperty("operation_id").GetString()!,
                root.GetProperty("module_name").GetString()!,
                root.GetProperty("module_key").GetString()!,
                root.GetProperty("path").GetString()!,
                root.GetProperty("method").GetString()!,
                root.GetProperty("auth_scope").GetString()!,
                root.TryGetProperty("access_token_kind", out var accessTokenKindElement)
                    ? accessTokenKindElement.GetString() ?? string.Empty
                    : string.Empty,
                root.GetProperty("request_content_kind").GetString()!,
                ReadHeaders(root),
                ReadParameters(root, "request_parameters"),
                ReadParameters(root, "response_parameters")));
        }

        return endpoints;
    }

    private static IReadOnlyList<SchemaParameter> ReadParameters(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var parameters) || parameters.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return parameters.EnumerateArray().Select(ReadParameter).ToArray();
    }

    private static SchemaParameter ReadParameter(JsonElement parameter)
    {
        var children = parameter.TryGetProperty("children", out var childElement) && childElement.ValueKind == JsonValueKind.Array
            ? childElement.EnumerateArray().Select(ReadParameter).ToArray()
            : [];

        return new SchemaParameter(
            parameter.GetProperty("name").GetString()!,
            parameter.GetProperty("type").GetString() ?? "object",
            parameter.GetProperty("required").GetBoolean(),
            parameter.TryGetProperty("location", out var locationElement) ? locationElement.GetString() ?? "body" : "body",
            children);
    }

    private static IReadOnlyList<string> ReadHeaders(JsonElement root)
    {
        if (!root.TryGetProperty("required_headers", out var headers) || headers.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return headers.EnumerateArray()
            .Select(static header => header.GetString())
            .Where(static header => !string.IsNullOrWhiteSpace(header))
            .Cast<string>()
            .ToArray();
    }
}
