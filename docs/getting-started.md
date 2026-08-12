# Getting Started

TikTokPartnerSdk targets .NET 8 and is published as preview packages under the `Loscardos.TikTokPartnerSdk.*` package IDs.

## Choose Packages

For ASP.NET Core applications, start with:

```bash
dotnet add package Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection --prerelease
```

Add EF Core token storage when you want the SDK to persist encrypted TikTok access and refresh tokens:

```bash
dotnet add package Loscardos.TikTokPartnerSdk.Storage.EntityFramework --prerelease
```

Use the lower-level packages directly when building custom hosts:

```bash
dotnet add package Loscardos.TikTokPartnerSdk.Core --prerelease
dotnet add package Loscardos.TikTokPartnerSdk.Generated --prerelease
```

## Configure ASP.NET Core

```csharp
using Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection;
using Loscardos.TikTokPartnerSdk.Storage.EntityFramework;

builder.Services.AddTikTokPartnerSdk(options =>
{
    options.AppKey = builder.Configuration["TikTok:AppKey"]!;
    options.AppSecret = builder.Configuration["TikTok:AppSecret"]!;
    options.EnableRateLimiting = true;
});

builder.Services.AddTikTokEntityFrameworkTokenStorage();
```

`AddTikTokPartnerSdk` registers:

- TikTok request signing and HTTP clients.
- Auth API and token refresh service.
- Token-aware order and product managers.
- Generated managers for the generated endpoint surface.
- Optional in-memory rate limiting.

For inbound TikTok Shop callbacks, read [Webhooks](webhooks.md) to wire a fast receiver with signature verification, timestamp validation, and app-owned queue handoff.

## Configuration

`TikTokPartnerOptions` supports:

| Option | Default | Notes |
| --- | --- | --- |
| `AppKey` | empty | Required for signed requests. |
| `AppSecret` | empty | Required for signed requests. Store securely. |
| `BaseUrl` | TikTok Open API host | Override for sandbox-compatible environments. |
| `AuthBaseUrl` | TikTok auth host | Override for token exchange and refresh. |
| `TokenRefreshSkew` | 5 minutes | Refresh tokens before expiry. |
| `RequestTimeout` | 30 seconds | Per request timeout. |
| `MaxTransientRetries` | 2 | Retry count for transient HTTP failures. |
| `RetryBaseDelay` | 250 ms | Base delay for exponential retry. |
| `EnableRateLimiting` | false | Enables SDK-side rate limiting. |
| `RateLimitPermitLimit` | 60 | Permit count per window. |
| `RateLimitWindow` | 1 minute | Rate limit window. |
| `UserAgent` | SDK default | Sent on SDK HTTP requests. |

## Next Steps

- Read [Authentication](authentication.md) to wire the OAuth callback.
- Read [Token Storage](token-storage.md) before going beyond local testing.
- Read [API Categories](api/README.md) to choose polished or generated managers.
