namespace TikTokPartnerSdk.Generator;

public sealed record YamlDocsEndpoint(
    string ModuleName,
    string ModuleKey,
    string Title,
    string Slug,
    string Path,
    string Method,
    IReadOnlyList<YamlDocsParameter> Headers,
    IReadOnlyList<YamlDocsParameter> PathParameters,
    IReadOnlyList<YamlDocsParameter> QueryParameters,
    IReadOnlyList<YamlDocsParameter> BodyParameters,
    IReadOnlyList<YamlDocsParameter> ResponseParameters);

public sealed record YamlDocsParameter(
    string Name,
    string Type,
    bool Required);
