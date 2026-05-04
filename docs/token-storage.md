# Token Storage

The SDK stores tokens through `ITikTokTokenStore`.

```csharp
using TikTokPartnerSdk.Abstractions.Auth;

public interface ITikTokTokenStore
{
    Task<TikTokTokenRecord?> GetAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken);
    Task StoreAsync(TikTokTokenRecord token, CancellationToken cancellationToken);
    Task ClearAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken);
}
```

## In-Memory Store

`AddTikTokPartnerSdk` registers an in-memory token store by default. It is useful for local tests and sample-console work, but not for production.

## EF Core Store

For ASP.NET Core applications, add:

```bash
dotnet add package Loscardos.TikTokPartnerSdk.Storage.EntityFramework --prerelease
```

Then register:

```csharp
using TikTokPartnerSdk.Storage.EntityFramework;

builder.Services.AddTikTokEntityFrameworkTokenStorage();
```

The EF store uses ASP.NET Data Protection for token protection. Configure Data Protection keys for your hosting environment before using it in production.

## Tenant Mapping

Persist the following values in your tenant or shop table:

| Value | Notes |
| --- | --- |
| `app_key` | Identifies the Partner app that owns the authorization. |
| `shop_cipher` | Required for most seller shop APIs. |
| `access_token_kind` | Usually `Seller` for shop integrations. |

