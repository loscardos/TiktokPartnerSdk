namespace Loscardos.TikTokPartnerSdk.Generator;

public sealed record SchemaModule(
    string Name,
    string Key,
    IReadOnlyList<SchemaEndpoint> Endpoints);
