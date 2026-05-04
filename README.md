# TikTokPartnerSdk

ASP.NET-friendly TikTok Shop Partner SDK for .NET 8.

TikTokPartnerSdk provides signed TikTok Shop Open API requests, authorization helpers, token refresh support, ASP.NET Core dependency injection, Entity Framework token storage, token-aware seller managers, and generated endpoint clients for the TikTok Partner API surface.

> Status: `0.1.0-preview`. The package is ready for sandbox validation and early integration work. Review behavior against your own TikTok app permissions before live rollout.

## Packages

| Package | Purpose |
| --- | --- |
| `Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection` | Recommended ASP.NET Core entrypoint. Registers the SDK, HTTP clients, auth, token service, polished managers, and generated managers. |
| `Loscardos.TikTokPartnerSdk.Core` | Core signing, HTTP pipeline, retries, rate limiting, token refresh, and manager implementations. |
| `Loscardos.TikTokPartnerSdk.Abstractions` | Public contracts for configuration, token stores, managers, errors, HTTP requests, and pagination. |
| `Loscardos.TikTokPartnerSdk.Generated` | Generated TikTok request and response DTO contracts. |
| `Loscardos.TikTokPartnerSdk.Storage.EntityFramework` | EF Core token store with ASP.NET Data Protection encryption. |

## Install

For an ASP.NET Core application:

```bash
dotnet add package Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection --prerelease
dotnet add package Loscardos.TikTokPartnerSdk.Storage.EntityFramework --prerelease
```

For lower-level consumers:

```bash
dotnet add package Loscardos.TikTokPartnerSdk.Core --prerelease
dotnet add package Loscardos.TikTokPartnerSdk.Generated --prerelease
```

## Quick Start

```csharp
using TikTokPartnerSdk.Extensions.DependencyInjection;
using TikTokPartnerSdk.Storage.EntityFramework;

builder.Services.AddTikTokPartnerSdk(options =>
{
    options.AppKey = builder.Configuration["TikTok:AppKey"]!;
    options.AppSecret = builder.Configuration["TikTok:AppSecret"]!;
    options.EnableRateLimiting = true;
});

builder.Services.AddTikTokEntityFrameworkTokenStorage();
```

Applications must register an `ITikTokTokenStore`. The EF storage package is provided for ASP.NET Core apps; custom services can implement `ITikTokTokenStore` directly.

## Auth Flow

```csharp
using TikTokPartnerSdk.Abstractions.Managers;

var authorizeUrl = authApi.BuildAuthorizationUrl(
    new Uri("https://example.com/tiktok/callback"),
    state: "csrf-state");
```

After TikTok redirects back with `code` and `shop_region`, exchange the code and associate the token with the authorized shop cipher:

```csharp
using TikTokPartnerSdk.Abstractions.Auth;

var context = new TikTokAuthorizationContext(
    TikTokAccessTokenKind.Seller,
    appKey,
    shopCipher);

var token = await authApi.ExchangeCodeAsync(
    code,
    context,
    cancellationToken);
```

Persisted tokens are used by token-aware managers and refreshed through `TikTokTokenService` when needed.

## Calling Endpoints

The SDK exposes two API surfaces:

- Polished managers for common seller workflows: orders and products.
- Generated managers for full SDK endpoint coverage. Generated managers live under `TikTokPartnerSdk.Abstractions.Managers.Generated` and use DTOs from `TikTokPartnerSdk.Generated.<Module>`.
- Webhooks primitives for raw payload verification, timestamp validation, idempotency keys, and typed event parsing.

Example using the token-aware order manager:

```csharp
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Managers;

public sealed class OrderSync(IOrderManager orders)
{
    public async Task SyncAsync(CancellationToken cancellationToken)
    {
        var context = new TikTokAuthorizationContext(
            TikTokAccessTokenKind.Seller,
            appKey,
            shopCipher);

        var page = await orders.SearchOrdersAsync(
            context,
            new TikTokOrderSearchRequest(PageSize: 50),
            cancellationToken);
    }
}
```

Example using a generated seller manager:

```csharp
using TikTokPartnerSdk.Generated.Seller;
using GeneratedSellerApi = TikTokPartnerSdk.Abstractions.Managers.Generated.ISellerApi;

public sealed class ShopSync(GeneratedSellerApi sellerApi)
{
    public Task<SellerGetActiveShopsResponse> GetActiveShopsAsync(
        string accessToken,
        string appKey,
        CancellationToken cancellationToken)
    {
        return sellerApi.GetActiveShopsAsync(
            accessToken,
            new SellerGetActiveShopsRequest(appKey, 0, string.Empty),
            cancellationToken);
    }
}
```

## Sample Validation CLI

The sample console can generate authorization URLs, exchange sandbox codes, refresh tokens, run read-only sandbox checks, and write endpoint certification reports.

```bash
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- check-config
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- auth-url
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- exchange-code
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- refresh-token
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- orders-search
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- products-search
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- smoke-readonly
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- certify-readonly
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- certify-matrix
```

Use ignored local files such as `.token/tiktok-sandbox.env` for sandbox credentials. Do not commit access tokens, refresh tokens, app secrets, auth codes, or shop credentials.

## Documentation

- [Getting Started](docs/getting-started.md)
- [Authentication](docs/authentication.md)
- [Token Storage](docs/token-storage.md)
- [Runtime Behavior](docs/runtime.md)
- [Sandbox Validation](docs/sandbox.md)
- [API Categories](docs/api/README.md)
- [Packaging](docs/packaging.md)

## Development

```bash
dotnet restore TikTokPartnerSdk.sln
dotnet build TikTokPartnerSdk.sln --no-restore
dotnet test TikTokPartnerSdk.sln --no-build
./scripts/verify-generated.sh
./scripts/verify-packages.sh
```

The CI workflow runs restore, build, test, generated SDK surface verification, endpoint certification matrix generation, and local NuGet install validation.

## License

MIT.
