# Product API

Use `IProductManager` for token-aware product search and detail workflows. Use generated `IProductApi` for full product endpoint coverage.

```csharp
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Managers;
```

## Polished Manager

```text
TikTokPartnerSdk.Abstractions.Managers.IProductManager
```

`IProductManager` supports:

- `SearchProductsAsync`
- `SearchAllProductsAsync`
- `GetProductAsync`

## Generated Interface

```text
TikTokPartnerSdk.Abstractions.Managers.Generated.IProductApi
```

## DTO Namespace

```text
TikTokPartnerSdk.Generated.Product
```

## Common Operations

- Category, brand, and attribute lookup.
- Product create, edit, partial edit, activate, deactivate, recover, and delete.
- Product search and detail.
- Inventory and price updates.
- Product image and file upload.
- Listing checks and diagnostics.

