namespace TikTokPartnerSdk.Abstractions.Errors;

public enum TikTokErrorCategory
{
    Unknown,
    Authentication,
    Signature,
    Validation,
    RateLimit,
    Transient
}
