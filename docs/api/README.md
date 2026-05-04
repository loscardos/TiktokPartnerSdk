# API Categories

The generated API surface is split by TikTok Shop category. Generated interfaces live under `TikTokPartnerSdk.Abstractions.Managers.Generated`; generated DTOs live under `TikTokPartnerSdk.Generated.<Category>`.

For token-aware seller workflows, prefer the polished managers where available:

- `IOrderManager`
- `IProductManager`

For full coverage, inject the generated category interface directly.

## Categories

| Category | Interface | DTO Namespace | Endpoints | Notes |
| --- | --- | --- | ---: | --- |
| [Affiliate Creator](affiliate.md#affiliate-creator) | `IAffiliateCreatorApi` | `TikTokPartnerSdk.Generated.AffiliateCreator` | 22 | Creator affiliate operations. |
| [Affiliate Partner](affiliate.md#affiliate-partner) | `IAffiliatePartnerApi` | `TikTokPartnerSdk.Generated.AffiliatePartner` | 15 | Partner affiliate campaign operations. |
| [Affiliate Seller](affiliate.md#affiliate-seller) | `IAffiliateSellerApi` | `TikTokPartnerSdk.Generated.AffiliateSeller` | 35 | Seller affiliate collaboration operations. |
| [Analytics](analytics.md) | `IAnalyticsApi` | `TikTokPartnerSdk.Generated.Analytics` | 26 | Shop, product, live, and video analytics. |
| [Authorization](authorization.md) | `IAuthorizationApi` | `TikTokPartnerSdk.Generated.Authorization` | 2 | Authorized shops and category assets. |
| [Customer Engagement](customer-engagement.md) | `ICustomerEngagementApi` | `TikTokPartnerSdk.Generated.CustomerEngagement` | 6 | Engagement tasks and message templates. |
| [Customer Service](customer-service.md) | `ICustomerServiceApi` | `TikTokPartnerSdk.Generated.CustomerService` | 11 | Conversations, messages, sessions, and support performance. |
| [Event](events.md) | `IEventApi` | `TikTokPartnerSdk.Generated.Event` | 3 | Shop webhook management. |
| [Finance](finance.md) | `IFinanceApi` | `TikTokPartnerSdk.Generated.Finance` | 6 | Payments, statements, withdrawals, and transactions. |
| [Fulfilled by TikTok](fbt.md) | `IFulfilledByTiktokFbtApi` | `TikTokPartnerSdk.Generated.FulfilledByTiktokFbt` | 22 | FBT merchant, goods, inventory, and inbound operations. |
| [Fulfillment](fulfillment.md) | `IFulfillmentApi` | `TikTokPartnerSdk.Generated.Fulfillment` | 25 | Packages, shipping, labels, tracking, and invoices. |
| [Logistics](logistics.md) | `ILogisticsApi` | `TikTokPartnerSdk.Generated.Logistics` | 5 | Warehouses, shipping providers, and templates. |
| [Order](orders.md) | `IOrderApi` | `TikTokPartnerSdk.Generated.Order` | 7 | Order search, details, price details, and references. |
| [Product](products.md) | `IProductApi` | `TikTokPartnerSdk.Generated.Product` | 59 | Catalog, product, inventory, price, images, and listing operations. |
| [Promotion](promotion.md) | `IPromotionApi` | `TikTokPartnerSdk.Generated.Promotion` | 9 | Activities and coupons. |
| [Return and Refund](returns-refunds.md) | `IReturnAndRefundApi` | `TikTokPartnerSdk.Generated.ReturnAndRefund` | 13 | Cancellations, returns, refunds, and eligibility. |
| [Seller](seller.md) | `ISellerApi` | `TikTokPartnerSdk.Generated.Seller` | 3 | Shops, permissions, and shop groups. |
| [Supply Chain](supply-chain.md) | `ISupplyChainApi` | `TikTokPartnerSdk.Generated.SupplyChain` | 1 | Package shipment confirmation. |
| [Tools](tools.md) | `IToolsApi` | `TikTokPartnerSdk.Generated.Tools` | 1 | File upload initialization. |

