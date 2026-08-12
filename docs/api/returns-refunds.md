# Return and Refund API

Use `IReturnAndRefundApi` for cancellation, return, refund, and eligibility workflows.

```csharp
using Loscardos.TikTokPartnerSdk.Generated.ReturnAndRefund;
using ReturnAndRefundApi = Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated.IReturnAndRefundApi;
```

## Generated Interface

```text
Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated.IReturnAndRefundApi
```

## DTO Namespace

```text
Loscardos.TikTokPartnerSdk.Generated.ReturnAndRefund
```

## Common Operations

- Search cancellations.
- Search returns.
- Get return records.
- Approve or reject cancellations.
- Approve or reject returns.
- Calculate refunds.
- Check aftersale and decision eligibility.

Write operations require sandbox fixtures with eligible cancellation or return states.

