namespace TikTokPartnerSdk.Generator;

public sealed record SchemaEndpoint(
    string OperationId,
    string ModuleName,
    string ModuleKey,
    string Path,
    string HttpMethod,
    string AuthScope,
    string AccessTokenKind,
    string RequestContentKind,
    IReadOnlyList<string> RequiredHeaders,
    IReadOnlyList<SchemaParameter> RequestParameters,
    IReadOnlyList<SchemaParameter> ResponseParameters)
{
    public string Title => OperationId;

    public string Slug => OperationId;

    public string Method => HttpMethod;

    public IReadOnlyList<SchemaField> QueryParams => RequestParameters
        .Select(static parameter => new SchemaField(parameter.Name, parameter.Type, parameter.Required))
        .ToArray();

    public IReadOnlyList<SchemaField> BodyParams => [];

    public IReadOnlyList<SchemaField> ResponseFields => ResponseParameters
        .Select(static parameter => new SchemaField(parameter.Name, parameter.Type, parameter.Required))
        .ToArray();
}
