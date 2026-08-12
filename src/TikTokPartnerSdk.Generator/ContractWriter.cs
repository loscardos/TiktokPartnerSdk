namespace Loscardos.TikTokPartnerSdk.Generator;

public sealed class ContractWriter
{
    public string WriteSummary(IReadOnlyList<SchemaEndpoint> endpoints)
    {
        return $"Total endpoints: {endpoints.Count}";
    }
}
