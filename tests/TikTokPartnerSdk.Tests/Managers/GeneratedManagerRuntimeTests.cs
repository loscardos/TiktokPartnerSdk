using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Core.Managers.Generated;
using TikTokPartnerSdk.Generated.Authorization;
using TikTokPartnerSdk.Generated.Event;
using TikTokPartnerSdk.Generated.Finance;
using TikTokPartnerSdk.Generated.Fulfillment;
using TikTokPartnerSdk.Generated.Logistics;
using TikTokPartnerSdk.Generated.Order;
using TikTokPartnerSdk.Generated.Product;
using TikTokPartnerSdk.Generated.ReturnAndRefund;
using TikTokPartnerSdk.Generated.Seller;

namespace TikTokPartnerSdk.Tests.Managers;

public sealed class GeneratedManagerRuntimeTests
{
    [Fact]
    public async Task Authorization_api_should_send_authorized_shops_request_with_access_token()
    {
        var client = new RecordingClient(
            new TikTokPartnerResponseEnvelope<AuthorizationGetAuthorizedShopsResponseData>(
                0,
                "success",
                "req-1",
                new AuthorizationGetAuthorizedShopsResponseData([])));
        var api = new AuthorizationApi(client);

        var response = await api.GetAuthorizedShopsAsync(
            "seller-token",
            new AuthorizationGetAuthorizedShopsRequest("app-key", 1, "sign"),
            CancellationToken.None);

        response.Code.Should().Be(0);
        client.LastRequest.Should().NotBeNull();
        client.LastRequest!.Method.Should().Be(HttpMethod.Get);
        client.LastRequest.Path.Should().Be("/authorization/202309/shops");
        client.LastRequest.AccessToken.Should().Be("seller-token");
    }

    [Fact]
    public async Task Authorization_api_should_map_envelope_data_response()
    {
        var data = new AuthorizationGetAuthorizedCategoryAssetsResponseData(
            [
                new AuthorizationGetAuthorizedCategoryAssetsResponseDataCategoryAssets(
                    "cipher-1",
                    "ID",
                    new AuthorizationGetAuthorizedCategoryAssetsResponseDataCategoryAssetsCategory(10, "Fashion"))
            ]);
        var client = new RecordingClient(
            new TikTokPartnerResponseEnvelope<AuthorizationGetAuthorizedCategoryAssetsResponseData>(
                0,
                "success",
                "req-2",
                data));
        var api = new AuthorizationApi(client);

        var response = await api.GetAuthorizedCategoryAssetsAsync(
            "partner-token",
            new AuthorizationGetAuthorizedCategoryAssetsRequest("app-key", 1, "sign"),
            CancellationToken.None);

        response.Data.CategoryAssets.Should().HaveCount(1);
        client.LastRequest!.Path.Should().Be("/authorization/202405/category_assets");
    }

    [Fact]
    public async Task Seller_and_event_apis_should_send_expected_paths()
    {
        var sellerClient = new RecordingClient(
            new TikTokPartnerResponseEnvelope<SellerGetActiveShopsResponseData>(
                0,
                "success",
                "req-3",
                new SellerGetActiveShopsResponseData([])));
        var sellerApi = new SellerApi(sellerClient);

        await sellerApi.GetActiveShopsAsync(
            "seller-token",
            new SellerGetActiveShopsRequest("app-key", 1, "sign"),
            CancellationToken.None);

        sellerClient.LastRequest!.Path.Should().Be("/seller/202309/shops");

        var eventClient = new RecordingClient(
            new TikTokPartnerResponseEnvelope<EventGetShopWebhooksResponseData>(
                0,
                "success",
                "req-4",
                new EventGetShopWebhooksResponseData([], 0)));
        var eventApi = new EventApi(eventClient);

        await eventApi.GetShopWebhooksAsync(
            "seller-token",
            new EventGetShopWebhooksRequest("app-key", 1, "sign", "shop-cipher"),
            CancellationToken.None);

        eventClient.LastRequest!.Path.Should().Be("/event/202309/webhooks");
        eventClient.LastRequest.Query.Should().ContainKey("shop_cipher")
            .WhoseValue.Should().Be("shop-cipher");
    }

    [Fact]
    public async Task Seller_api_should_send_permissions_and_shop_group_paths()
    {
        var permissionsClient = new RecordingClient(
            new TikTokPartnerResponseEnvelope<SellerGetSellerPermissionsResponseData>(
                0,
                "success",
                "req-5",
                new SellerGetSellerPermissionsResponseData([])));
        var permissionsApi = new SellerApi(permissionsClient);

        await permissionsApi.GetSellerPermissionsAsync(
            "seller-token",
            new SellerGetSellerPermissionsRequest("app-key", 1, "sign"),
            CancellationToken.None);

        permissionsClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        permissionsClient.LastRequest.Path.Should().Be("/seller/202309/permissions");

        var shopGroupClient = new RecordingClient(
            new TikTokPartnerResponseEnvelope<SellerGetShopGroupResponseData>(
                0,
                "success",
                "req-6",
                new SellerGetShopGroupResponseData(
                    new SellerGetShopGroupResponseDataShopGroupData(
                        new SellerGetShopGroupResponseDataShopGroupDataShopGroup("SYNC", "Group"),
                        []))));
        var shopGroupApi = new SellerApi(shopGroupClient);

        await shopGroupApi.GetShopGroupAsync(
            "seller-token",
            new SellerGetShopGroupRequest("app-key", 1, "sign"),
            CancellationToken.None);

        shopGroupClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        shopGroupClient.LastRequest.Path.Should().Be("/seller/202601/shop_groups");
    }

    [Fact]
    public async Task Event_api_should_send_delete_and_update_body_payloads()
    {
        var deleteClient = new RecordingClient(
            new TikTokPartnerResponseEnvelope<object>(
                0,
                "success",
                "req-7",
                new { }));
        var deleteApi = new EventApi(deleteClient);

        await deleteApi.DeleteShopWebhookAsync(
            "seller-token",
            new EventDeleteShopWebhookRequest("app-key", 1, "sign", "shop-cipher", "ORDER_STATUS_CHANGE"),
            CancellationToken.None);

        deleteClient.LastRequest!.Method.Should().Be(HttpMethod.Delete);
        deleteClient.LastRequest.Path.Should().Be("/event/202309/webhooks");
        deleteClient.LastRequest.Query.Should().ContainKey("shop_cipher")
            .WhoseValue.Should().Be("shop-cipher");
        deleteClient.LastRequest.Body.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            ["event_type"] = "ORDER_STATUS_CHANGE"
        });

        var updateClient = new RecordingClient(
            new TikTokPartnerResponseEnvelope<object>(
                0,
                "success",
                "req-8",
                new { }));
        var updateApi = new EventApi(updateClient);

        await updateApi.UpdateShopWebhookAsync(
            "seller-token",
            new EventUpdateShopWebhookRequest("app-key", 1, "sign", "shop-cipher", "https://example.test/webhook", "ORDER_STATUS_CHANGE"),
            CancellationToken.None);

        updateClient.LastRequest!.Method.Should().Be(HttpMethod.Put);
        updateClient.LastRequest.Path.Should().Be("/event/202309/webhooks");
        updateClient.LastRequest.Query.Should().ContainKey("shop_cipher")
            .WhoseValue.Should().Be("shop-cipher");
        updateClient.LastRequest.Body.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            ["address"] = "https://example.test/webhook",
            ["event_type"] = "ORDER_STATUS_CHANGE"
        });
    }

    [Fact]
    public async Task Order_api_should_send_core_order_paths()
    {
        var detailClient = new RecordingClient();
        var detailApi = new OrderApi(detailClient);

        await detailApi.GetOrderDetailAsync(
            "seller-token",
            new OrderGetOrderDetailRequest("app-key", 1, "sign", ["order-1"], "shop-cipher"),
            CancellationToken.None);

        detailClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        detailClient.LastRequest.Path.Should().Be("/order/202507/orders");
        detailClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        detailClient.LastRequest.Query.Should().ContainKey("ids");

        var listClient = new RecordingClient();
        var listApi = new OrderApi(listClient);

        await listApi.GetOrderListAsync(
            "seller-token",
            new OrderGetOrderListRequest(
                "app-key",
                1,
                "sign",
                20,
                string.Empty,
                "shop-cipher",
                "update_time",
                "DESC",
                "UNPAID",
                1710000000,
                1710003600,
                1710000000,
                1710003600,
                "TIKTOK",
                "buyer-1",
                false,
                []),
            CancellationToken.None);

        listClient.LastRequest!.Method.Should().Be(HttpMethod.Post);
        listClient.LastRequest.Path.Should().Be("/order/202309/orders/search");
        listClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        listClient.LastRequest.Body.Should().NotBeNull();

        var priceClient = new RecordingClient();
        var priceApi = new OrderApi(priceClient);

        await priceApi.GetPriceDetailAsync(
            "seller-token",
            new OrderGetPriceDetailRequest("order-1", "app-key", 1, "sign", "shop-cipher"),
            CancellationToken.None);

        priceClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        priceClient.LastRequest.Path.Should().Be("/order/202407/orders/order-1/price_detail");
        priceClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
    }

    [Fact]
    public async Task Product_api_should_send_core_catalog_paths()
    {
        var categoriesClient = new RecordingClient();
        var categoriesApi = new ProductApi(categoriesClient);

        await categoriesApi.GetCategoriesAsync(
            "seller-token",
            new ProductGetCategoriesRequest(
                "app-key",
                1,
                "sign",
                "v2",
                true,
                "shirt",
                "TikTok Shop",
                "en",
                "shop-cipher"),
            CancellationToken.None);

        categoriesClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        categoriesClient.LastRequest.Path.Should().Be("/product/202309/categories");
        categoriesClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");

        var productClient = new RecordingClient();
        var productApi = new ProductApi(productClient);

        await productApi.GetProductAsync(
            "seller-token",
            new ProductGetProductRequest(
                "product-1",
                "app-key",
                1,
                "sign",
                "en",
                false,
                false,
                "shop-cipher"),
            CancellationToken.None);

        productClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        productClient.LastRequest.Path.Should().Be("/product/202309/products/product-1");
        productClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");

        var brandsClient = new RecordingClient();
        var brandsApi = new ProductApi(brandsClient);

        await brandsApi.GetBrandsAsync(
            "seller-token",
            new ProductGetBrandsRequest(
                "app-key",
                1,
                "sign",
                "Acme",
                "category-1",
                "v2",
                true,
                20,
                string.Empty,
                "shop-cipher"),
            CancellationToken.None);

        brandsClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        brandsClient.LastRequest.Path.Should().Be("/product/202309/brands");
        brandsClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
    }

    [Fact]
    public async Task Fulfillment_api_should_send_core_package_paths()
    {
        var detailClient = new RecordingClient();
        var detailApi = new FulfillmentApi(detailClient);

        await detailApi.GetPackageDetailAsync(
            "seller-token",
            new FulfillmentGetPackageDetailRequest("package-1", "app-key", 1, "sign", "shop-cipher"),
            CancellationToken.None);

        detailClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        detailClient.LastRequest.Path.Should().Be("/fulfillment/202309/packages/package-1");
        detailClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");

        var trackingClient = new RecordingClient();
        var trackingApi = new FulfillmentApi(trackingClient);

        await trackingApi.GetTrackingAsync(
            "seller-token",
            new FulfillmentGetTrackingRequest("order-1", "app-key", 1, "sign", "shop-cipher"),
            CancellationToken.None);

        trackingClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        trackingClient.LastRequest.Path.Should().Be("/fulfillment/202309/orders/order-1/tracking");
        trackingClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");

        var shipClient = new RecordingClient();
        var shipApi = new FulfillmentApi(shipClient);

        await shipApi.ShipPackageAsync(
            "seller-token",
            new FulfillmentShipPackageRequest(
                "package-1",
                "app-key",
                1,
                "sign",
                "shop-cipher",
                "PICKUP",
                new FulfillmentShipPackageRequestPickupSlot(1710000000, 1710003600),
                new FulfillmentShipPackageRequestSelfShipment("track-1", "provider-1")),
            CancellationToken.None);

        shipClient.LastRequest!.Method.Should().Be(HttpMethod.Post);
        shipClient.LastRequest.Path.Should().Be("/fulfillment/202309/packages/package-1/ship");
        shipClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        shipClient.LastRequest.Body.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            ["handover_method"] = "PICKUP",
            ["pickup_slot"] = new FulfillmentShipPackageRequestPickupSlot(1710000000, 1710003600),
            ["self_shipment"] = new FulfillmentShipPackageRequestSelfShipment("track-1", "provider-1")
        });
    }

    [Fact]
    public async Task Logistics_api_should_send_core_warehouse_paths()
    {
        var warehousesClient = new RecordingClient();
        var warehousesApi = new LogisticsApi(warehousesClient);

        await warehousesApi.GetWarehouseListAsync(
            "seller-token",
            new LogisticsGetWarehouseListRequest("app-key", 1, "sign", "shop-cipher"),
            CancellationToken.None);

        warehousesClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        warehousesClient.LastRequest.Path.Should().Be("/logistics/202309/warehouses");
        warehousesClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");

        var providersClient = new RecordingClient();
        var providersApi = new LogisticsApi(providersClient);

        await providersApi.GetShippingProvidersAsync(
            "seller-token",
            new LogisticsGetShippingProvidersRequest(
                "delivery-option-1",
                "app-key",
                1,
                "sign",
                "ID",
                "shop-cipher",
                "ID"),
            CancellationToken.None);

        providersClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        providersClient.LastRequest.Path.Should().Be("/logistics/202309/delivery_options/delivery-option-1/shipping_providers");
        providersClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        providersClient.LastRequest.Query.Should().ContainKey("buyer_region").WhoseValue.Should().Be("ID");
        providersClient.LastRequest.Query.Should().ContainKey("warehouse_region").WhoseValue.Should().Be("ID");
    }

    [Fact]
    public async Task Return_refund_api_should_send_core_aftersale_paths()
    {
        var recordsClient = new RecordingClient();
        var recordsApi = new ReturnAndRefundApi(recordsClient);

        await recordsApi.GetReturnRecordsAsync(
            "seller-token",
            new ReturnAndRefundGetReturnRecordsRequest("return-1", "app-key", 1, "sign", "en", "shop-cipher"),
            CancellationToken.None);

        recordsClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        recordsClient.LastRequest.Path.Should().Be("/return_refund/202309/returns/return-1/records");
        recordsClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        recordsClient.LastRequest.Query.Should().ContainKey("locale").WhoseValue.Should().Be("en");

        var approveClient = new RecordingClient();
        var approveApi = new ReturnAndRefundApi(approveClient);

        await approveApi.ApproveReturnAsync(
            "seller-token",
            new ReturnAndRefundApproveReturnRequest(
                "return-1",
                "app-key",
                1,
                "sign",
                "idempotency-1",
                "shop-cipher",
                "APPROVE",
                false,
                new ReturnAndRefundApproveReturnRequestPartialRefund("IDR", "1000")),
            CancellationToken.None);

        approveClient.LastRequest!.Method.Should().Be(HttpMethod.Post);
        approveClient.LastRequest.Path.Should().Be("/return_refund/202309/returns/return-1/approve");
        approveClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        approveClient.LastRequest.Query.Should().ContainKey("idempotency_key").WhoseValue.Should().Be("idempotency-1");
        approveClient.LastRequest.Body.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            ["decision"] = "APPROVE",
            ["buyer_keep_item"] = false,
            ["partial_refund"] = new ReturnAndRefundApproveReturnRequestPartialRefund("IDR", "1000")
        });

        var eligibilityClient = new RecordingClient();
        var eligibilityApi = new ReturnAndRefundApi(eligibilityClient);

        await eligibilityApi.GetAftersaleEligibilityAsync(
            "seller-token",
            new ReturnAndRefundGetAftersaleEligibilityRequest(
                "order-1",
                "app-key",
                1,
                "sign",
                "SELLER",
                ["REFUND", "RETURN_AND_REFUND"],
                "shop-cipher"),
            CancellationToken.None);

        eligibilityClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        eligibilityClient.LastRequest.Path.Should().Be("/return_refund/202602/orders/order-1/aftersale_eligibility");
        eligibilityClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        eligibilityClient.LastRequest.Query.Should().ContainKey("initiate_aftersale_user").WhoseValue.Should().Be("SELLER");
        eligibilityClient.LastRequest.Query.Should().ContainKey("request_types");
    }

    [Fact]
    public async Task Finance_api_should_send_core_reconciliation_paths()
    {
        var paymentsClient = new RecordingClient();
        var paymentsApi = new FinanceApi(paymentsClient);

        await paymentsApi.GetPaymentsAsync(
            "seller-token",
            new FinanceGetPaymentsRequest(
                "app-key",
                1,
                "sign",
                1710000000,
                1710003600,
                20,
                string.Empty,
                "shop-cipher",
                "create_time",
                "DESC"),
            CancellationToken.None);

        paymentsClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        paymentsClient.LastRequest.Path.Should().Be("/finance/202309/payments");
        paymentsClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        paymentsClient.LastRequest.Query.Should().ContainKey("create_time_ge").WhoseValue.Should().Be(1710000000);

        var orderTransactionsClient = new RecordingClient();
        var orderTransactionsApi = new FinanceApi(orderTransactionsClient);

        await orderTransactionsApi.GetTransactionsByOrderAsync(
            "seller-token",
            new FinanceGetTransactionsByOrderRequest("order-1", "app-key", 1, "sign", "shop-cipher"),
            CancellationToken.None);

        orderTransactionsClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        orderTransactionsClient.LastRequest.Path.Should().Be("/finance/202501/orders/order-1/statement_transactions");
        orderTransactionsClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");

        var statementTransactionsClient = new RecordingClient();
        var statementTransactionsApi = new FinanceApi(statementTransactionsClient);

        await statementTransactionsApi.GetTransactionsByStatementAsync(
            "seller-token",
            new FinanceGetTransactionsByStatementRequest(
                "statement-1",
                "app-key",
                1,
                "sign",
                20,
                string.Empty,
                "shop-cipher",
                "transaction_time",
                "DESC"),
            CancellationToken.None);

        statementTransactionsClient.LastRequest!.Method.Should().Be(HttpMethod.Get);
        statementTransactionsClient.LastRequest.Path.Should().Be("/finance/202501/statements/statement-1/statement_transactions");
        statementTransactionsClient.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
    }

    private sealed class RecordingClient(object response) : ITikTokPartnerClient
    {
        public RecordingClient()
            : this(new object())
        {
        }

        public TikTokPartnerRequest? LastRequest { get; private set; }

        public Task<TikTokPartnerResponseEnvelope<TResponse>> SendAsync<TResponse>(
            TikTokPartnerRequest request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (response is TikTokPartnerResponseEnvelope<TResponse> envelope)
            {
                return Task.FromResult(envelope);
            }

            return Task.FromResult(new TikTokPartnerResponseEnvelope<TResponse>(
                0,
                "success",
                "req-generated",
                CreatePayload<TResponse>()));
        }

        private static TResponse CreatePayload<TResponse>()
        {
            if (typeof(TResponse) == typeof(object))
            {
                return (TResponse)new object();
            }

            return (TResponse)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(TResponse));
        }
    }
}
