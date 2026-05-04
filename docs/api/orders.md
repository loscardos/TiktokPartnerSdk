# Order API

Use `IOrderManager` for token-aware order search and detail workflows. Use generated `IOrderApi` when you need direct endpoint coverage.

```csharp
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Managers;
```

## Polished Manager

```text
TikTokPartnerSdk.Abstractions.Managers.IOrderManager
```

`IOrderManager` supports:

- `SearchOrdersAsync`
- `SearchAllOrdersAsync`
- `GetOrderDetailAsync`

## Generated Interface

```text
TikTokPartnerSdk.Abstractions.Managers.Generated.IOrderApi
```

## DTO Namespace

```text
TikTokPartnerSdk.Generated.Order
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

