namespace TikTokPartnerSdk.Abstractions.Http;

public sealed record TikTokPartnerResponseEnvelope<TData>(
    int Code,
    string Message,
    string? RequestId,
    TData? Data);
