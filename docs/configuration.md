# Configuration

`TikTokPartnerOptions` is configured through `AddTikTokPartnerSdk`.

```csharp
builder.Services.AddTikTokPartnerSdk(options =>
{
    options.AppKey = builder.Configuration["TikTok:AppKey"]!;
    options.AppSecret = builder.Configuration["TikTok:AppSecret"]!;
    options.EnableRateLimiting = true;
});
```

## Required Values

| Option | Description |
| --- | --- |
| `AppKey` | TikTok Partner app key. |
| `AppSecret` | TikTok Partner app secret used for request signatures. |

## Runtime Controls

| Option | Description |
| --- | --- |
| `RequestTimeout` | Maximum duration for one HTTP request. |
| `MaxTransientRetries` | Retry count for timeout, rate-limit, and server errors. |
| `RetryBaseDelay` | Base delay used by retry backoff. |
| `TokenRefreshSkew` | How early token-aware managers refresh tokens. |
| `EnableRateLimiting` | Enables local in-memory request limiting. |
| `RateLimitPermitLimit` | Number of permits per rate-limit window. |
| `RateLimitWindow` | Duration of one rate-limit window. |
| `UserAgent` | User agent sent by SDK HTTP clients. |

## Base URLs

The default base URLs target TikTok Shop Partner API hosts. Override them only for sandbox-compatible environments, proxies, or controlled integration tests.

