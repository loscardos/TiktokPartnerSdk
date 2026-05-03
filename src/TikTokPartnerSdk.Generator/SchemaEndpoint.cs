namespace TikTokPartnerSdk.Generator;

public sealed record SchemaEndpoint(
    string Title,
    string Slug,
    string Method,
    string Path,
    IReadOnlyList<SchemaField> QueryParams,
    IReadOnlyList<SchemaField> BodyParams,
    IReadOnlyList<SchemaField> ResponseFields);
