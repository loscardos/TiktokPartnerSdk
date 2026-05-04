# Runtime Behavior

## Signing

The SDK signs Open API requests with TikTok's HMAC-SHA256 signing rules. The signer excludes `sign` and `access_token` from the canonical query string.

## HTTP Pipeline

`ITikTokPartnerClient` handles:

- Query string construction.
- JSON request body serialization.
- Multipart request body support where generated request contracts require it.
- `x-tts-access-token` header injection.
- Response parsing into `TikTokPartnerResponseEnvelope<T>`.
- Typed API exceptions.

## Retries

Transient HTTP failures are retried according to `MaxTransientRetries` and `RetryBaseDelay`.

The SDK retries:

- HTTP 408.
- HTTP 429.
- HTTP 5xx.
- Request timeout failures when the caller cancellation token was not cancelled.

## Rate Limiting

Local rate limiting is disabled by default. Enable it when you want the SDK to smooth traffic before it reaches TikTok:

```csharp
builder.Services.AddTikTokPartnerSdk(options =>
{
    options.EnableRateLimiting = true;
    options.RateLimitPermitLimit = 60;
    options.RateLimitWindow = TimeSpan.FromMinutes(1);
});
```

## Error Handling

TikTok API failures are surfaced as `TikTokApiException`. The exception includes the HTTP status, TikTok message, request id when available, and a coarse error category.

