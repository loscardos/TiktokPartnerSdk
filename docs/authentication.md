# Authentication

TikTok Shop Partner API uses an authorization code flow. The SDK provides `IAuthApi` for building authorization URLs, exchanging authorization codes, refreshing tokens, and storing token results.

## Build the Authorization URL

```csharp
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;

var redirectUri = new Uri("https://example.com/tiktok/callback");
var url = authApi.BuildAuthorizationUrl(redirectUri, state: "csrf-state");
```

Send the seller to `url`. After authorization, TikTok redirects back with `code`, `shop_region`, and `state`.

## Exchange Code

TikTok access tokens are stored against a `TikTokAuthorizationContext`. For seller APIs, include the authorized shop cipher.

```csharp
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;

var context = new TikTokAuthorizationContext(
    TikTokAccessTokenKind.Seller,
    appKey,
    shopCipher);

var token = await authApi.ExchangeCodeAsync(
    code,
    context,
    cancellationToken);
```

`ExchangeCodeAsync` stores the token through the configured `ITikTokTokenStore`.

## Refresh Token

```csharp
var refreshed = await authApi.RefreshTokenAsync(
    context,
    cancellationToken);
```

Token-aware managers use `TikTokTokenService` to refresh tokens when they are inside the configured refresh skew.

## Production Notes

- Store `AppSecret`, access tokens, and refresh tokens in secure infrastructure.
- Do not place shop access tokens in application settings for production.
- Persist `shop_cipher` with the tenant record that owns the authorization.
- Treat authorization codes as one-time credentials.
- Rotate sandbox app secrets if they were shared in logs, tickets, or chat.

## Local Sandbox Environment

For local sample-console testing, use ignored `.token` files:

```bash
TIKTOK_SANDBOX_APP_KEY=
TIKTOK_SANDBOX_APP_SECRET=
TIKTOK_SANDBOX_REDIRECT_URL=
TIKTOK_SANDBOX_AUTH_CODE=
TIKTOK_SANDBOX_SHOP_CIPHER=
TIKTOK_SANDBOX_ACCESS_TOKEN=
TIKTOK_SANDBOX_REFRESH_TOKEN=
```

The sample console masks token values when printing them.

