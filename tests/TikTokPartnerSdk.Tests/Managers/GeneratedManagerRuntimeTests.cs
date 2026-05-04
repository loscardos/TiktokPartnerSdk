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
            new TikTokPartnerResponseEnvelope<object>(
                0,
                "success",
                "req-1",
                null));
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
            new TikTokPartnerResponseEnvelope<object>(
                0,
                "success",
                "req-3",
                null));
        var sellerApi = new SellerApi(sellerClient);

        await sellerApi.GetActiveShopsAsync(
            "seller-token",
            new SellerGetActiveShopsRequest("app-key", 1, "sign"),
            CancellationToken.None);

        sellerClient.LastRequest!.Path.Should().Be("/seller/202309/shops");

        var eventClient = new RecordingClient(
            new TikTokPartnerResponseEnvelope<object>(
                0,
                "success",
                "req-4",
                null));
        var eventApi = new EventApi(eventClient);

        await eventApi.GetShopWebhooksAsync(
            "seller-token",
            new EventGetShopWebhooksRequest("app-key", 1, "sign"),
            CancellationToken.None);

        eventClient.LastRequest!.Path.Should().Be("/event/202309/webhooks");
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
