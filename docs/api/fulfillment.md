# Fulfillment API

Use `IFulfillmentApi` for package, shipping, tracking, document, and invoice operations.

```csharp
using TikTokPartnerSdk.Generated.Fulfillment;
using FulfillmentApi = TikTokPartnerSdk.Abstractions.Managers.Generated.IFulfillmentApi;
```

## Generated Interface

```text
TikTokPartnerSdk.Abstractions.Managers.Generated.IFulfillmentApi
```

## DTO Namespace

```text
TikTokPartnerSdk.Generated.Fulfillment
```

## Common Operations

- Search packages.
- Get package detail.
- Ship, split, combine, and uncombine packages.
- Get shipping services and handover slots.
- Get shipping documents.
- Upload delivery files, delivery images, and invoices.

Mutation endpoints require sandbox fixtures with eligible package states.

