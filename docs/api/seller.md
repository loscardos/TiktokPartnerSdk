# Seller API

Use `ISellerApi` for seller shop metadata and permissions.

```csharp
using TikTokPartnerSdk.Generated.Seller;
using SellerApi = TikTokPartnerSdk.Abstractions.Managers.Generated.ISellerApi;
```

## Generated Interface

```text
TikTokPartnerSdk.Abstractions.Managers.Generated.ISellerApi
```

## DTO Namespace

```text
TikTokPartnerSdk.Generated.Seller
```

## Common Operations

- Get active shops.
- Get seller permissions.
- Get shop group.

Seller shop APIs usually require a seller token and, for shop-scoped operations, the authorized `shop_cipher`.

