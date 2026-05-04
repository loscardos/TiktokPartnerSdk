using FluentAssertions;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Core.Auth;
using TikTokPartnerSdk.Core.Managers;
using TikTokPartnerSdk.Generated.Order;

namespace TikTokPartnerSdk.Tests.Managers;

public sealed class OrderManagerTests
{
    [Fact]
    public async Task SearchOrdersAsync_should_resolve_token_and_return_page()
    {
        var token = CreateToken();
        var orderApi = new RecordingOrderApi(
            new OrderGetOrderListResponse(
                0,
                "success",
                "req-1",
                new OrderGetOrderListResponseData("next", 12, [])));
        var manager = CreateManager(orderApi, token);

        var page = await manager.SearchOrdersAsync(
            CreateContext(),
            new TikTokOrderSearchRequest(PageSize: 25, PageToken: "start", OrderStatus: "UNPAID"),
            CancellationToken.None);

        page.NextPageToken.Should().Be("next");
        page.TotalCount.Should().Be(12);
        orderApi.AccessToken.Should().Be("seller-access");
        orderApi.ListRequest.Should().NotBeNull();
        orderApi.ListRequest!.PageSize.Should().Be(25);
        orderApi.ListRequest.PageToken.Should().Be("start");
        orderApi.ListRequest.ShopCipher.Should().Be("shop-cipher");
        orderApi.ListRequest.OrderStatus.Should().Be("UNPAID");
    }

    [Fact]
    public async Task GetOrderDetailAsync_should_use_context_shop_cipher()
    {
        var token = CreateToken();
        var orderApi = new RecordingOrderApi(
            new OrderGetOrderDetailResponse(
                0,
                "success",
                "req-2",
                new OrderGetOrderDetailResponseData([])));
        var manager = CreateManager(orderApi, token);

        await manager.GetOrderDetailAsync(
            CreateContext(),
            ["order-1", "order-2"],
            CancellationToken.None);

        orderApi.AccessToken.Should().Be("seller-access");
        orderApi.DetailRequest.Should().NotBeNull();
        orderApi.DetailRequest!.Ids.Should().Equal("order-1", "order-2");
        orderApi.DetailRequest.ShopCipher.Should().Be("shop-cipher");
    }

    private static OrderManager CreateManager(RecordingOrderApi orderApi, TikTokTokenRecord token)
        => new(
            orderApi,
            new TikTokTokenService(
                new InMemoryTokenStore(token),
                new StubAuthApi(token),
                Options.Create(new TikTokPartnerOptions())));

    private static TikTokAuthorizationContext CreateContext()
        => new(TikTokAccessTokenKind.Seller, "app-key", "shop-cipher");

    private static TikTokTokenRecord CreateToken()
        => new(
            TikTokAccessTokenKind.Seller,
            "seller-access",
            "refresh",
            DateTimeOffset.UtcNow.AddHours(1),
            DateTimeOffset.UtcNow.AddDays(30),
            "shop-cipher",
            "app-key");

    private sealed class RecordingOrderApi(object response) : IOrderApi
    {
        public string? AccessToken { get; private set; }

        public OrderGetOrderListRequest? ListRequest { get; private set; }

        public OrderGetOrderDetailRequest? DetailRequest { get; private set; }

        public Task<OrderGetOrderListResponse> GetOrderListAsync(
            string accessToken,
            OrderGetOrderListRequest request,
            CancellationToken cancellationToken)
        {
            AccessToken = accessToken;
            ListRequest = request;
            return Task.FromResult((OrderGetOrderListResponse)response);
        }

        public Task<OrderGetOrderDetailResponse> GetOrderDetailAsync(
            string accessToken,
            OrderGetOrderDetailRequest request,
            CancellationToken cancellationToken)
        {
            AccessToken = accessToken;
            DetailRequest = request;
            return Task.FromResult((OrderGetOrderDetailResponse)response);
        }

        public Task<OrderAddExternalOrderReferencesResponse> AddExternalOrderReferencesAsync(string accessToken, OrderAddExternalOrderReferencesRequest request, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<OrderGetExternalOrderReferencesResponse> GetExternalOrderReferencesAsync(string accessToken, OrderGetExternalOrderReferencesRequest request, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<OrderSearchOrderByExternalOrderReferenceResponse> SearchOrderByExternalOrderReferenceAsync(string accessToken, OrderSearchOrderByExternalOrderReferenceRequest request, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<OrderGetPriceDetailResponse> GetPriceDetailAsync(string accessToken, OrderGetPriceDetailRequest request, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<OrderUpdateTheBlindBoxOpeningResultsResponse> UpdateTheBlindBoxOpeningResultsAsync(string accessToken, OrderUpdateTheBlindBoxOpeningResultsRequest request, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }

    private sealed class InMemoryTokenStore(TikTokTokenRecord token) : ITikTokTokenStore
    {
        public Task<TikTokTokenRecord?> GetAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
            => Task.FromResult<TikTokTokenRecord?>(token);

        public Task StoreAsync(TikTokTokenRecord token, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task ClearAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    private sealed class StubAuthApi(TikTokTokenRecord token) : IAuthApi
    {
        public Uri BuildAuthorizationUrl(Uri redirectUri, string? state)
            => throw new NotSupportedException();

        public Task<TikTokTokenRecord> ExchangeCodeAsync(string code, TikTokAuthorizationContext context, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<TikTokTokenRecord> RefreshTokenAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
            => Task.FromResult(token);
    }
}
