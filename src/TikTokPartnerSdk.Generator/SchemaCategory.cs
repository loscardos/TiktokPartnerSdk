namespace TikTokPartnerSdk.Generator;

public sealed record SchemaCategory(
    string Name,
    string FileName,
    IReadOnlyList<SchemaEndpoint> Endpoints);
