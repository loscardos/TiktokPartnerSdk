# Product API

Use `IProductManager` for token-aware product search and detail workflows. Use generated `IProductApi` for full product endpoint coverage.

```csharp
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;
```

## Polished Manager

```text
Loscardos.TikTokPartnerSdk.Abstractions.Managers.IProductManager
```

`IProductManager` supports:

- `SearchProductsAsync`
- `SearchAllProductsAsync`
- `GetProductAsync`

## Generated Interface

```text
Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated.IProductApi
```

## DTO Namespace

```text
Loscardos.TikTokPartnerSdk.Generated.Product
```

## Common Operations

- Category, brand, and attribute lookup.
- Product create, edit, partial edit, activate, deactivate, recover, and delete.
- Product search and detail.
- Inventory and price updates.
- Product image and file upload.
- Listing checks and diagnostics.

