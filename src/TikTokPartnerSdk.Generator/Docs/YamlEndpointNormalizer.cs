using System.Text.RegularExpressions;

namespace TikTokPartnerSdk.Generator;

public sealed partial class YamlEndpointNormalizer
{
    public SchemaEndpoint Normalize(YamlDocsEndpoint endpoint)
    {
        var moduleKey = CanonicalModuleKey(endpoint.ModuleKey);
        var moduleName = CanonicalModuleName(endpoint.ModuleName, moduleKey);
        var requestParameters = endpoint.PathParameters.Select(static parameter => ToSchemaParameter(parameter, "path"))
            .Concat(OrderRequestParameters(endpoint.QueryParameters).Select(static parameter => ToSchemaParameter(parameter, "query")))
            .Concat(BuildParameterTree(endpoint.BodyParameters, "body"))
            .ToArray();

        return new SchemaEndpoint(
            ToOperationId(endpoint, moduleKey),
            moduleName,
            moduleKey,
            endpoint.Path,
            endpoint.Method,
            InferAccessTokenKind(endpoint),
            InferAccessTokenKind(endpoint),
            endpoint.BodyParameters.Count > 0 ? "body" : "query",
            OrderHeaders(endpoint.Headers).Select(static header => header.Name).ToArray(),
            requestParameters,
            BuildParameterTree(endpoint.ResponseParameters, "body"));
    }

    private static SchemaParameter ToSchemaParameter(YamlDocsParameter parameter, string location)
        => new(CleanName(parameter.Name), parameter.Type, parameter.Required, location, []);

    private static IEnumerable<YamlDocsParameter> OrderRequestParameters(IEnumerable<YamlDocsParameter> parameters)
        => parameters.OrderBy(static parameter => parameter.Name switch
        {
            "app_key" => 0,
            "timestamp" => 1,
            "sign" => 2,
            _ => 100
        }).ThenBy(static parameter => parameter.Name, StringComparer.Ordinal);

    private static IEnumerable<YamlDocsParameter> OrderHeaders(IEnumerable<YamlDocsParameter> parameters)
        => parameters.OrderBy(static parameter => parameter.Name switch
        {
            "x-tts-access-token" => 0,
            "content-type" => 1,
            _ => 100
        }).ThenBy(static parameter => parameter.Name, StringComparer.Ordinal);

    private static IReadOnlyList<SchemaParameter> BuildParameterTree(
        IReadOnlyList<YamlDocsParameter> parameters,
        string location)
    {
        var roots = new List<MutableSchemaParameter>();
        var stack = new List<MutableSchemaParameter>();

        foreach (var parameter in parameters)
        {
            var level = parameter.Name.TakeWhile(static character => character == '^').Count();
            var node = new MutableSchemaParameter(CleanName(parameter.Name), parameter.Type, parameter.Required, location);

            if (level == 0 || stack.Count == 0)
            {
                roots.Add(node);
            }
            else
            {
                var parentIndex = Math.Min(level - 1, stack.Count - 1);
                stack[parentIndex].Children.Add(node);
            }

            if (stack.Count <= level)
            {
                stack.Add(node);
            }
            else
            {
                stack[level] = node;
                if (stack.Count > level + 1)
                {
                    stack.RemoveRange(level + 1, stack.Count - level - 1);
                }
            }
        }

        return roots.Select(static root => root.ToSchemaParameter()).ToArray();
    }

    private static string CleanName(string name) => name.TrimStart('^');

    private static string InferAccessTokenKind(YamlDocsEndpoint endpoint)
    {
        if (endpoint.ModuleKey.Equals("authorization", StringComparison.Ordinal)
            && endpoint.Path.Contains("category_assets", StringComparison.OrdinalIgnoreCase))
        {
            return "partner";
        }

        return "seller";
    }

    private static string CanonicalModuleKey(string moduleKey)
        => moduleKey switch
        {
            "orders" => "order",
            "products" => "product",
            _ => moduleKey
        };

    private static string CanonicalModuleName(string moduleName, string moduleKey)
        => moduleKey switch
        {
            "order" => "Order",
            "product" => "Product",
            _ => moduleName
        };

    private static string ToOperationId(YamlDocsEndpoint endpoint, string moduleKey)
    {
        var version = endpoint.Path.Split('/', StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault() ?? "unknown";
        var operationName = endpoint.Slug;
        if (operationName.EndsWith("-" + version, StringComparison.Ordinal))
        {
            operationName = operationName[..^(version.Length + 1)];
        }

        operationName = NonIdentifierCharacterRegex().Replace(operationName, "_").Trim('_');
        return $"{moduleKey}.{version}.{operationName}";
    }

    [GeneratedRegex("[^a-zA-Z0-9]+")]
    private static partial Regex NonIdentifierCharacterRegex();

    private sealed class MutableSchemaParameter(
        string name,
        string type,
        bool required,
        string location)
    {
        public List<MutableSchemaParameter> Children { get; } = [];

        public SchemaParameter ToSchemaParameter()
            => new(name, type, required, location, Children.Select(static child => child.ToSchemaParameter()).ToArray());
    }
}
