# Order API

Use `IOrderManager` for token-aware order search and detail workflows. Use generated `IOrderApi` when you need direct endpoint coverage.

```csharp
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;
```

## Polished Manager

```text
Loscardos.TikTokPartnerSdk.Abstractions.Managers.IOrderManager
```

`IOrderManager` supports:

- `SearchOrdersAsync`
- `SearchAllOrdersAsync`
- `GetOrderDetailAsync`

## Generated Interface

```text
Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated.IOrderApi
```

## DTO Namespace

```text
Loscardos.TikTokPartnerSdk.Generated.Order
```

## Example

```csharp
var context = new TikTokAuthorizationContext(
    TikTokAccessTokenKind.Seller,
    appKey,
    shopCipher);

var page = await orderManager.SearchOrdersAsync(
    context,
    new TikTokOrderSearchRequest(PageSize: 50),
    cancellationToken);
```

