using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated;
using Loscardos.TikTokPartnerSdk.Abstractions.Pagination;
using Loscardos.TikTokPartnerSdk.Core.Auth;
using Loscardos.TikTokPartnerSdk.Generated.Order;

namespace Loscardos.TikTokPartnerSdk.Core.Managers;

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
            response.Data.Orders ?? Array.Empty<OrderGetOrderListResponseDataOrders>(),
            response.Data.NextPageToken ?? string.Empty,
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
            Optional(request.OrderStatus)!,
            request.CreateTimeGe,
            request.CreateTimeLt,
            request.UpdateTimeGe,
            request.UpdateTimeLt,
            Optional(request.ShippingType)!,
            Optional(request.BuyerUserId)!,
            request.IsBuyerRequestCancel,
            request.WarehouseIds ?? Array.Empty<string>());

    private static string? Optional(string value)
        => string.IsNullOrWhiteSpace(value) ? null : value;
}
