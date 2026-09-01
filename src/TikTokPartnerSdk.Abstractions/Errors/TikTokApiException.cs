namespace Loscardos.TikTokPartnerSdk.Abstractions.Errors;

public sealed class TikTokApiException : Exception
{
    public TikTokApiException(
        long code,
        string message,
        string? requestId,
        TikTokErrorCategory category,
        TimeSpan? retryAfter = null)
        : base($"TikTok API error {code}: {message}")
    {
        Code = code;
        RequestId = requestId;
        Category = category;
        RetryAfter = retryAfter;
    }

    public long Code { get; }

    public string? RequestId { get; }

    public TikTokErrorCategory Category { get; }

    public TimeSpan? RetryAfter { get; }
}
