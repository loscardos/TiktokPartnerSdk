namespace TikTokPartnerSdk.Generator;

public sealed class YamlApiReferenceReader
{
    public IReadOnlyList<YamlDocsEndpoint> ReadDirectory(string directory)
    {
        return Directory.EnumerateFiles(directory, "*.yaml")
            .OrderBy(static path => path, StringComparer.Ordinal)
            .SelectMany(ReadFile)
            .ToArray();
    }

    public IReadOnlyList<YamlDocsEndpoint> ReadFile(string path)
    {
        var moduleKey = Path.GetFileNameWithoutExtension(path);
        var moduleName = TikTokName.ToPascalCase(moduleKey);
        var endpoints = new List<YamlDocsEndpoint>();
        var builder = new EndpointBuilder(moduleName, moduleKey);
        var section = YamlParameterSection.None;
        MutableParameter? currentParameter = null;

        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.TrimEnd();
            var trimmed = line.TrimStart();

            if (line.StartsWith("  name:", StringComparison.Ordinal))
            {
                moduleName = ExtractQuotedValue(line);
                builder.ModuleName = moduleName;
                continue;
            }

            if (line.StartsWith("    - title:", StringComparison.Ordinal))
            {
                FlushParameter();
                FlushEndpoint();
                builder = new EndpointBuilder(moduleName, moduleKey)
                {
                    Title = ExtractQuotedValue(line)
                };
                section = YamlParameterSection.None;
                continue;
            }

            if (builder.Title.Length == 0)
            {
                continue;
            }

            if (line.StartsWith("      slug:", StringComparison.Ordinal))
            {
                builder.Slug = ExtractQuotedValue(line);
                continue;
            }

            if (line.StartsWith("      method:", StringComparison.Ordinal))
            {
                builder.Method = ExtractQuotedValue(line);
                continue;
            }

            if (line.StartsWith("      path:", StringComparison.Ordinal))
            {
                builder.Path = ExtractQuotedValue(line);
                continue;
            }

            if (line.StartsWith("        headers:", StringComparison.Ordinal))
            {
                FlushParameter();
                section = YamlParameterSection.Header;
                continue;
            }

            if (line.StartsWith("        path_params:", StringComparison.Ordinal))
            {
                FlushParameter();
                section = YamlParameterSection.Path;
                continue;
            }

            if (line.StartsWith("        query_params:", StringComparison.Ordinal))
            {
                FlushParameter();
                section = YamlParameterSection.Query;
                continue;
            }

            if (line.StartsWith("        body_params:", StringComparison.Ordinal))
            {
                FlushParameter();
                section = YamlParameterSection.Body;
                continue;
            }

            if (line.StartsWith("        fields:", StringComparison.Ordinal))
            {
                FlushParameter();
                section = YamlParameterSection.Response;
                continue;
            }

            if (line.StartsWith("      samples:", StringComparison.Ordinal)
                || line.StartsWith("        error_codes:", StringComparison.Ordinal)
                || line.StartsWith("      response:", StringComparison.Ordinal)
                || line.StartsWith("      request:", StringComparison.Ordinal))
            {
                FlushParameter();
                if (!line.StartsWith("      request:", StringComparison.Ordinal)
                    && !line.StartsWith("      response:", StringComparison.Ordinal))
                {
                    section = YamlParameterSection.None;
                }
                continue;
            }

            if (section == YamlParameterSection.None)
            {
                continue;
            }

            if (trimmed.StartsWith("- name:", StringComparison.Ordinal))
            {
                FlushParameter();
                currentParameter = new MutableParameter
                {
                    Name = ExtractQuotedValue(trimmed),
                    Type = "object"
                };
                continue;
            }

            if (currentParameter is null)
            {
                continue;
            }

            if (trimmed.StartsWith("type:", StringComparison.Ordinal))
            {
                currentParameter.Type = ExtractQuotedValue(trimmed);
                continue;
            }

            if (trimmed.StartsWith("required:", StringComparison.Ordinal))
            {
                currentParameter.Required = ExtractQuotedValue(trimmed).Equals("Y", StringComparison.OrdinalIgnoreCase);
            }
        }

        FlushParameter();
        FlushEndpoint();
        return endpoints;

        void FlushParameter()
        {
            if (currentParameter is null)
            {
                return;
            }

            builder.Add(section, currentParameter.ToParameter());
            currentParameter = null;
        }

        void FlushEndpoint()
        {
            if (string.IsNullOrWhiteSpace(builder.Path))
            {
                return;
            }

            endpoints.Add(builder.Build());
        }
    }

    private static string ExtractQuotedValue(string line)
    {
        var separator = line.IndexOf(':', StringComparison.Ordinal);
        if (separator < 0)
        {
            return string.Empty;
        }

        var value = line[(separator + 1)..].Trim();
        if (value.Length >= 2 && value[0] == '\'' && value[^1] == '\'')
        {
            value = value[1..^1];
        }

        return value.Replace("''", "'", StringComparison.Ordinal);
    }

    private enum YamlParameterSection
    {
        None,
        Header,
        Path,
        Query,
        Body,
        Response
    }

    private sealed class MutableParameter
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "object";
        public bool Required { get; set; }

        public YamlDocsParameter ToParameter() => new(Name, Type, Required);
    }

    private sealed class EndpointBuilder(string moduleName, string moduleKey)
    {
        private readonly List<YamlDocsParameter> _headers = [];
        private readonly List<YamlDocsParameter> _pathParameters = [];
        private readonly List<YamlDocsParameter> _queryParameters = [];
        private readonly List<YamlDocsParameter> _bodyParameters = [];
        private readonly List<YamlDocsParameter> _responseParameters = [];

        public string ModuleName { get; set; } = moduleName;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;

        public void Add(YamlParameterSection section, YamlDocsParameter parameter)
        {
            switch (section)
            {
                case YamlParameterSection.Header:
                    _headers.Add(parameter);
                    break;
                case YamlParameterSection.Path:
                    _pathParameters.Add(parameter);
                    break;
                case YamlParameterSection.Query:
                    _queryParameters.Add(parameter);
                    break;
                case YamlParameterSection.Body:
                    _bodyParameters.Add(parameter);
                    break;
                case YamlParameterSection.Response:
                    _responseParameters.Add(parameter);
                    break;
            }
        }

        public YamlDocsEndpoint Build()
            => new(
                ModuleName,
                moduleKey,
                Title,
                Slug,
                Path,
                Method,
                _headers.ToArray(),
                _pathParameters.ToArray(),
                _queryParameters.ToArray(),
                _bodyParameters.ToArray(),
                _responseParameters.ToArray());
    }
}
