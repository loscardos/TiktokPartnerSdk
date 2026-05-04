# Return and Refund API

Use `IReturnAndRefundApi` for cancellation, return, refund, and eligibility workflows.

```csharp
using TikTokPartnerSdk.Generated.ReturnAndRefund;
using ReturnAndRefundApi = TikTokPartnerSdk.Abstractions.Managers.Generated.IReturnAndRefundApi;
```

## Generated Interface

```text
TikTokPartnerSdk.Abstractions.Managers.Generated.IReturnAndRefundApi
```

## DTO Namespace

```text
TikTokPartnerSdk.Generated.ReturnAndRefund
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

