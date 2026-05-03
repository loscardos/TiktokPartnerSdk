namespace TikTokPartnerSdk.Generator;

public sealed class SchemaReader
{
    public SchemaCategory ReadCategory(string yamlPath)
    {
        var lines = File.ReadAllLines(yamlPath);
        var categoryName = string.Empty;
        var endpoints = new List<SchemaEndpoint>();

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var trimmed = line.Trim();

            if (line.StartsWith("  name:", StringComparison.Ordinal) && categoryName.Length == 0)
            {
                categoryName = Unquote(ReadValue(trimmed));
                continue;
            }

            if (!trimmed.StartsWith("- title:", StringComparison.Ordinal))
            {
                continue;
            }

            var title = Unquote(ReadValue(trimmed));
            string slug = string.Empty;
            string method = string.Empty;
            string path = string.Empty;

            for (index += 1; index < lines.Length; index++)
            {
                var endpointLine = lines[index];
                var endpointTrimmed = endpointLine.Trim();

                if (endpointTrimmed.StartsWith("- title:", StringComparison.Ordinal))
                {
                    index -= 1;
                    break;
                }

                if (endpointLine.StartsWith("    - ", StringComparison.Ordinal))
                {
                    index -= 1;
                    break;
                }

                if (endpointTrimmed.StartsWith("slug:", StringComparison.Ordinal))
                {
                    slug = Unquote(ReadValue(endpointTrimmed));
                }
                else if (endpointTrimmed.StartsWith("method:", StringComparison.Ordinal))
                {
                    method = Unquote(ReadValue(endpointTrimmed));
                }
                else if (endpointTrimmed.StartsWith("path:", StringComparison.Ordinal))
                {
                    path = Unquote(ReadValue(endpointTrimmed));
                }
            }

            endpoints.Add(new SchemaEndpoint(
                title,
                slug,
                method,
                path,
                [],
                [],
                []));
        }

        if (categoryName.Length == 0)
        {
            throw new InvalidOperationException("Category name was not found in YAML.");
        }

        return new SchemaCategory(
            categoryName,
            Path.GetFileName(yamlPath),
            endpoints);
    }

    private static string ReadValue(string line)
    {
        var separatorIndex = line.IndexOf(':', StringComparison.Ordinal);
        if (separatorIndex < 0)
        {
            return string.Empty;
        }

        return line[(separatorIndex + 1)..].Trim();
    }

    private static string Unquote(string value)
    {
        return value.Trim().Trim('\'', '"');
    }
}
