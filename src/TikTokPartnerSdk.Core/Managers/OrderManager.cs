using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Abstractions.Pagination;
using TikTokPartnerSdk.Core.Auth;
using TikTokPartnerSdk.Generated.Order;

namespace TikTokPartnerSdk.Core.Managers;

public sealed class OrderManager(
    IOrderApi orderApi,
    TikTokTokenService tokenService) : IOrderManager
{
    public async Task<TikTokPage<OrderGetOrderListResponseDataOrders>> SearchOrdersAsync(
        TikTokAuthorizationContext context,
        TikTokOrderSearchRequest request,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetValidTokenAsync(context, cancellationToken).ConfigureAwait(false);
        var response = await orderApi.GetOrderListAsync(
            token.AccessToken,
            ToGeneratedRequest(context, request),
            cancellationToken).ConfigureAwait(false);

        return new TikTokPage<OrderGetOrderListResponseDataOrders>(
            response.Data.Orders,
            response.Data.NextPageToken,
            response.Data.TotalCount);
    }

    public IAsyncEnumerable<OrderGetOrderListResponseDataOrders> SearchAllOrdersAsync(
        TikTokAuthorizationContext context,
        TikTokOrderSearchRequest request,
        CancellationToken cancellationToken = default)
        => TikTokPagedEnumerable.ReadAllAsync(
            new TikTokPageRequest(request.PageSize, request.PageToken),
            (page, token) => SearchOrdersAsync(
                context,
                request with { PageSize = page.PageSize, PageToken = page.PageToken },
                token),
            cancellationToken);

    public async Task<OrderGetOrderDetailResponseData> GetOrderDetailAsync(
        TikTokAuthorizationContext context,
        IReadOnlyList<string> orderIds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderIds);

        var token = await tokenService.GetValidTokenAsync(context, cancellationToken).ConfigureAwait(false);
        var response = await orderApi.GetOrderDetailAsync(
            token.AccessToken,
            new OrderGetOrderDetailRequest(
                context.AppKey,
                0,
                string.Empty,
                orderIds,
                TikTokManagerContext.RequireShopCipher(context)),
            cancellationToken).ConfigureAwait(false);

        return response.Data;
    }

    private static OrderGetOrderListRequest ToGeneratedRequest(
        TikTokAuthorizationContext context,
        TikTokOrderSearchRequest request)
        => new(
            context.AppKey,
            0,
            string.Empty,
            request.PageSize,
            request.PageToken,
            TikTokManagerContext.RequireShopCipher(context),
            request.SortField,
            request.SortOrder,
            request.OrderStatus,
            request.CreateTimeGe,
            request.CreateTimeLt,
            request.UpdateTimeGe,
            request.UpdateTimeLt,
            request.ShippingType,
            request.BuyerUserId,
            request.IsBuyerRequestCancel,
            request.WarehouseIds ?? Array.Empty<string>());
}
