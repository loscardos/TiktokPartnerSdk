namespace TikTokPartnerSdk.Abstractions.Errors;

public enum TikTokErrorCategory
{
    Unknown,
    Authentication,
    Authorization,
    Signature,
    Validation,
    RateLimit,
    Transient
}
