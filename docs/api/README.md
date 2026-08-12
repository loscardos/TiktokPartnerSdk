# API Categories

The generated API surface is split by TikTok Shop category. Generated interfaces live under `Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated`; generated DTOs live under `Loscardos.TikTokPartnerSdk.Generated.<Category>`.

For token-aware seller workflows, prefer the polished managers where available:

- `IOrderManager`
- `IProductManager`

For full coverage, inject the generated category interface directly.

## Categories

| Category | Interface | DTO Namespace | Endpoints | Notes |
| --- | --- | --- | ---: | --- |
| [Affiliate Creator](affiliate.md#affiliate-creator) | `IAffiliateCreatorApi` | `Loscardos.TikTokPartnerSdk.Generated.AffiliateCreator` | 22 | Creator affiliate operations. |
| [Affiliate Partner](affiliate.md#affiliate-partner) | `IAffiliatePartnerApi` | `Loscardos.TikTokPartnerSdk.Generated.AffiliatePartner` | 15 | Partner affiliate campaign operations. |
| [Affiliate Seller](affiliate.md#affiliate-seller) | `IAffiliateSellerApi` | `Loscardos.TikTokPartnerSdk.Generated.AffiliateSeller` | 35 | Seller affiliate collaboration operations. |
| [Analytics](analytics.md) | `IAnalyticsApi` | `Loscardos.TikTokPartnerSdk.Generated.Analytics` | 26 | Shop, product, live, and video analytics. |
| [Authorization](authorization.md) | `IAuthorizationApi` | `Loscardos.TikTokPartnerSdk.Generated.Authorization` | 2 | Authorized shops and category assets. |
| [Customer Engagement](customer-engagement.md) | `ICustomerEngagementApi` | `Loscardos.TikTokPartnerSdk.Generated.CustomerEngagement` | 6 | Engagement tasks and message templates. |
| [Customer Service](customer-service.md) | `ICustomerServiceApi` | `Loscardos.TikTokPartnerSdk.Generated.CustomerService` | 11 | Conversations, messages, sessions, and support performance. |
| [Event](events.md) | `IEventApi` | `Loscardos.TikTokPartnerSdk.Generated.Event` | 3 | Shop webhook management. |
| [Finance](finance.md) | `IFinanceApi` | `Loscardos.TikTokPartnerSdk.Generated.Finance` | 6 | Payments, statements, withdrawals, and transactions. |
| [Fulfilled by TikTok](fbt.md) | `IFulfilledByTiktokFbtApi` | `Loscardos.TikTokPartnerSdk.Generated.FulfilledByTiktokFbt` | 22 | FBT merchant, goods, inventory, and inbound operations. |
| [Fulfillment](fulfillment.md) | `IFulfillmentApi` | `Loscardos.TikTokPartnerSdk.Generated.Fulfillment` | 25 | Packages, shipping, labels, tracking, and invoices. |
| [Logistics](logistics.md) | `ILogisticsApi` | `Loscardos.TikTokPartnerSdk.Generated.Logistics` | 5 | Warehouses, shipping providers, and templates. |
| [Order](orders.md) | `IOrderApi` | `Loscardos.TikTokPartnerSdk.Generated.Order` | 7 | Order search, details, price details, and references. |
| [Product](products.md) | `IProductApi` | `Loscardos.TikTokPartnerSdk.Generated.Product` | 59 | Catalog, product, inventory, price, images, and listing operations. |
| [Promotion](promotion.md) | `IPromotionApi` | `Loscardos.TikTokPartnerSdk.Generated.Promotion` | 9 | Activities and coupons. |
| [Return and Refund](returns-refunds.md) | `IReturnAndRefundApi` | `Loscardos.TikTokPartnerSdk.Generated.ReturnAndRefund` | 13 | Cancellations, returns, refunds, and eligibility. |
| [Seller](seller.md) | `ISellerApi` | `Loscardos.TikTokPartnerSdk.Generated.Seller` | 3 | Shops, permissions, and shop groups. |
| [Supply Chain](supply-chain.md) | `ISupplyChainApi` | `Loscardos.TikTokPartnerSdk.Generated.SupplyChain` | 1 | Package shipment confirmation. |
| [Tools](tools.md) | `IToolsApi` | `Loscardos.TikTokPartnerSdk.Generated.Tools` | 1 | File upload initialization. |

