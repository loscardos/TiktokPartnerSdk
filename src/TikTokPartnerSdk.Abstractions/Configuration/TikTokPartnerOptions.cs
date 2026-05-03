namespace TikTokPartnerSdk.Abstractions.Configuration;

public sealed class TikTokPartnerOptions
{
    public string AppKey { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string OpenApiBaseUrl { get; set; } = "https://open-api.tiktokglobalshop.com";
    public string AuthApiBaseUrl { get; set; } = "https://auth.tiktok-shops.com/api/v2";
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan TokenRefreshSkew { get; set; } = TimeSpan.FromMinutes(5);
    public string UserAgent { get; set; } = "InternalTikTokPartnerSdk/0.1";
}
