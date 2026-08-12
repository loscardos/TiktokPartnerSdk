using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Pagination;
using Loscardos.TikTokPartnerSdk.Generated.Order;

namespace Loscardos.TikTokPartnerSdk.Abstractions.Managers;

public interface IOrderManager
{
    Task<TikTokPage<OrderGetOrderListResponseDataOrders>> SearchOrdersAsync(
        TikTokAuthorizationContext context,
        TikTokOrderSearchRequest request,
        CancellationToken cancellationToken);

    IAsyncEnumerable<OrderGetOrderListResponseDataOrders> SearchAllOrdersAsync(
        TikTokAuthorizationContext context,
        TikTokOrderSearchRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderGetOrderDetailResponseData> GetOrderDetailAsync(
        TikTokAuthorizationContext context,
        IReadOnlyList<string> orderIds,
        CancellationToken cancellationToken);
}

public sealed record TikTokOrderSearchRequest(
    long PageSize = 50,
    string PageToken = "",
    string SortField = "update_time",
    string SortOrder = "DESC",
    string OrderStatus = "",
    long CreateTimeGe = 0,
    long CreateTimeLt = 0,
    long UpdateTimeGe = 0,
    long UpdateTimeLt = 0,
    string ShippingType = "",
    string BuyerUserId = "",
    bool IsBuyerRequestCancel = false,
    IReadOnlyList<string>? WarehouseIds = null);
