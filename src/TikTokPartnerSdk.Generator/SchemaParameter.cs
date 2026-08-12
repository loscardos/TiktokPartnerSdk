namespace Loscardos.TikTokPartnerSdk.Generator;

public sealed record SchemaParameter(
    string Name,
    string Type,
    bool Required,
    string Location,
    IReadOnlyList<SchemaParameter> Children);
