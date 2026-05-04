using System.Text;

namespace TikTokPartnerSdk.Generator;

public static class TikTokName
{
    public static string ToPascalCase(string value)
    {
        var parts = value
            .Split(new[] { '.', '_', '-', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var builder = new StringBuilder();

        foreach (var part in parts)
        {
            if (part.Length == 0)
            {
                continue;
            }

            builder.Append(char.ToUpperInvariant(part[0]));
            if (part.Length > 1)
            {
                builder.Append(part[1..]);
            }
        }

        return builder.ToString();
    }

    public static string ToPropertyName(string value)
        => ToPascalCase(value);

    public static string ToPropertyName(SchemaParameter parameter, IReadOnlyList<SchemaParameter> siblings)
    {
        var propertyName = ToPropertyName(parameter.Name);
        var duplicateName = siblings.Count(sibling => sibling.Name.Equals(parameter.Name, StringComparison.Ordinal)) > 1;
        return duplicateName
            ? ToPascalCase(parameter.Location) + propertyName
            : propertyName;
    }

    public static string ToRequestTypeName(SchemaEndpoint endpoint)
        => ToPascalCase(endpoint.ModuleKey) + ToOperationName(endpoint.OperationId) + "Request";

    public static string ToResponseTypeName(SchemaEndpoint endpoint)
        => ToPascalCase(endpoint.ModuleKey) + ToOperationName(endpoint.OperationId) + "Response";

    public static string ToManagerInterfaceName(string moduleKey)
        => "I" + ToPascalCase(moduleKey) + "Api";

    public static string ToManagerImplementationName(string moduleKey)
        => ToPascalCase(moduleKey) + "Api";

    public static string ToManagerMethodName(SchemaEndpoint endpoint)
        => ToOperationName(endpoint.OperationId);

    public static string ToChildTypeName(string parentTypeName, string parameterName)
        => parentTypeName + ToPascalCase(parameterName);

    private static string ToOperationName(string operationId)
    {
        var parts = operationId.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var relevant = parts.Length >= 3
            ? parts[2]
            : operationId;
        return ToPascalCase(relevant);
    }
}
