namespace TikTokPartnerSdk.Abstractions.Configuration;

public sealed class TikTokPartnerOptions
{
    public string AppKey { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string OpenApiBaseUrl { get; set; } = "https://open-api.tiktokglobalshop.com";
    public string AuthApiBaseUrl { get; set; } = "https://auth.tiktok-shops.com/api/v2";
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan TokenRefreshSkew { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxTransientRetries { get; set; } = 2;
    public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromMilliseconds(200);
    public bool EnableRateLimiting { get; set; }
    public int RateLimitPermitLimit { get; set; } = 60;
    public int RateLimitQueueLimit { get; set; } = 120;
    public TimeSpan RateLimitWindow { get; set; } = TimeSpan.FromMinutes(1);
    public string UserAgent { get; set; } = "InternalTikTokPartnerSdk/0.1";
}
