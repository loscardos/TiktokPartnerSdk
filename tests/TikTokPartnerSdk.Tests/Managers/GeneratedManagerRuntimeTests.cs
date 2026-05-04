using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Core.Managers.Generated;
using TikTokPartnerSdk.Generated.Authorization;
using TikTokPartnerSdk.Generated.Event;
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

    private sealed class RecordingClient(object response) : ITikTokPartnerClient
    {
        public TikTokPartnerRequest? LastRequest { get; private set; }

        public Task<TikTokPartnerResponseEnvelope<TResponse>> SendAsync<TResponse>(
            TikTokPartnerRequest request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult((TikTokPartnerResponseEnvelope<TResponse>)response);
        }
    }
}
