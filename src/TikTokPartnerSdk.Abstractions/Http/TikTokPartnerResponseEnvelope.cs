using System.Text.Json.Serialization;

namespace Loscardos.TikTokPartnerSdk.Abstractions.Http;

public sealed record TikTokPartnerResponseEnvelope<TData>(
    [property: JsonPropertyName("code")]
    int Code,
    [property: JsonPropertyName("message")]
    string Message,
    [property: JsonPropertyName("request_id")]
    string? RequestId,
    [property: JsonPropertyName("data")]
    TData? Data);
