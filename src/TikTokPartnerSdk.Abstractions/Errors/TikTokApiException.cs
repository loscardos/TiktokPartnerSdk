namespace Loscardos.TikTokPartnerSdk.Abstractions.Errors;

public sealed class TikTokApiException : Exception
{
    public TikTokApiException(long code, string message, string? requestId, TikTokErrorCategory category)
        : base($"TikTok API error {code}: {message}")
    {
        Code = code;
        RequestId = requestId;
        Category = category;
    }

    public long Code { get; }

    public string? RequestId { get; }

    public TikTokErrorCategory Category { get; }
}
