using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Pagination;
using TikTokPartnerSdk.Generated.Product;

namespace TikTokPartnerSdk.Abstractions.Managers;

public interface IProductManager
{
    Task<ProductGetProductResponseData> GetProductAsync(
        TikTokAuthorizationContext context,
        string productId,
        TikTokProductDetailOptions options,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductGetCategoriesResponseDataCategories>> GetCategoriesAsync(
        TikTokAuthorizationContext context,
        TikTokCategoryQuery query,
        CancellationToken cancellationToken);

    Task<TikTokPage<ProductGetBrandsResponseDataBrands>> GetBrandsAsync(
        TikTokAuthorizationContext context,
        TikTokBrandSearchRequest request,
        CancellationToken cancellationToken);

    Task<TikTokPage<ProductSearchProductsResponseDataProducts>> SearchProductsAsync(
        TikTokAuthorizationContext context,
        TikTokProductSearchRequest request,
        CancellationToken cancellationToken);

    IAsyncEnumerable<ProductSearchProductsResponseDataProducts> SearchAllProductsAsync(
        TikTokAuthorizationContext context,
        TikTokProductSearchRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record TikTokProductDetailOptions(
    string Locale = "",
    bool ReturnDraftVersion = false,
    bool ReturnUnderReviewVersion = false);

public sealed record TikTokCategoryQuery(
    string CategoryVersion = "",
    bool IncludeProhibitedCategories = false,
    string Keyword = "",
    string ListingPlatform = "",
    string Locale = "");

public sealed record TikTokBrandSearchRequest(
    long PageSize = 50,
    string PageToken = "",
    string BrandName = "",
    string CategoryId = "",
    string CategoryVersion = "",
    bool IsAuthorized = false);

public sealed record TikTokProductSearchRequest(
    long PageSize = 50,
    string PageToken = "",
    string Status = "",
    IReadOnlyList<string>? SellerSkus = null,
    long CreateTimeGe = 0,
    long CreateTimeLe = 0,
    long UpdateTimeGe = 0,
    long UpdateTimeLe = 0,
    string CategoryVersion = "",
    IReadOnlyList<string>? ListingQualityTiers = null,
    IReadOnlyList<string>? ListingPlatforms = null,
    IReadOnlyList<string>? AuditStatus = null,
    IReadOnlyList<string>? SkuIds = null,
    string SnsFilter = "",
    bool ReturnDraftVersion = false);
