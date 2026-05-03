namespace TikTokPartnerSdk.Generator;

public static class TikTokTypeMapper
{
    public static string MapScalar(string schemaType)
    {
        return schemaType switch
        {
            "string" => "string",
            "int" => "long",
            "integer" => "long",
            "bool" => "bool",
            "boolean" => "bool",
            _ => "string"
        };
    }

    public static string MapType(SchemaParameter parameter, string parentTypeName)
    {
        if (parameter.Children.Count == 0)
        {
            return MapScalar(parameter.Type);
        }

        var childTypeName = TikTokName.ToChildTypeName(parentTypeName, parameter.Name);
        return parameter.Type.StartsWith("[]", StringComparison.Ordinal)
            ? $"IReadOnlyList<{childTypeName}>"
            : childTypeName;
    }
}
