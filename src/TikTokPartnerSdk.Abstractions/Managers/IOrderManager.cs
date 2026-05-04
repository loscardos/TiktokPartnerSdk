using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Pagination;
using TikTokPartnerSdk.Generated.Order;

namespace TikTokPartnerSdk.Abstractions.Managers;

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
