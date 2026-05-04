using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Abstractions.Pagination;
using TikTokPartnerSdk.Core.Auth;
using TikTokPartnerSdk.Generated.Product;

namespace TikTokPartnerSdk.Core.Managers;

public sealed class ProductManager(
    IProductApi productApi,
    TikTokTokenService tokenService) : IProductManager
{
    public async Task<ProductGetProductResponseData> GetProductAsync(
        TikTokAuthorizationContext context,
        string productId,
        TikTokProductDetailOptions options,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetValidTokenAsync(context, cancellationToken).ConfigureAwait(false);
        var response = await productApi.GetProductAsync(
            token.AccessToken,
            new ProductGetProductRequest(
                productId,
                context.AppKey,
                0,
                string.Empty,
                options.Locale,
                options.ReturnDraftVersion,
                options.ReturnUnderReviewVersion,
                TikTokManagerContext.RequireShopCipher(context)),
            cancellationToken).ConfigureAwait(false);

        return response.Data;
    }

    public async Task<IReadOnlyList<ProductGetCategoriesResponseDataCategories>> GetCategoriesAsync(
        TikTokAuthorizationContext context,
        TikTokCategoryQuery query,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetValidTokenAsync(context, cancellationToken).ConfigureAwait(false);
        var response = await productApi.GetCategoriesAsync(
            token.AccessToken,
            new ProductGetCategoriesRequest(
                context.AppKey,
                0,
                string.Empty,
                query.CategoryVersion,
                query.IncludeProhibitedCategories,
                query.Keyword,
                query.ListingPlatform,
                query.Locale,
                TikTokManagerContext.RequireShopCipher(context)),
            cancellationToken).ConfigureAwait(false);

        return response.Data.Categories;
    }

    public async Task<TikTokPage<ProductGetBrandsResponseDataBrands>> GetBrandsAsync(
        TikTokAuthorizationContext context,
        TikTokBrandSearchRequest request,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetValidTokenAsync(context, cancellationToken).ConfigureAwait(false);
        var response = await productApi.GetBrandsAsync(
            token.AccessToken,
            new ProductGetBrandsRequest(
                context.AppKey,
                0,
                string.Empty,
                request.BrandName,
                request.CategoryId,
                request.CategoryVersion,
                request.IsAuthorized,
                request.PageSize,
                request.PageToken,
                TikTokManagerContext.RequireShopCipher(context)),
            cancellationToken).ConfigureAwait(false);

        return new TikTokPage<ProductGetBrandsResponseDataBrands>(
            response.Data.Brands,
            response.Data.NextPageToken,
            response.Data.TotalCount);
    }

    public async Task<TikTokPage<ProductSearchProductsResponseDataProducts>> SearchProductsAsync(
        TikTokAuthorizationContext context,
        TikTokProductSearchRequest request,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetValidTokenAsync(context, cancellationToken).ConfigureAwait(false);
        var response = await productApi.SearchProductsAsync(
            token.AccessToken,
            ToGeneratedRequest(context, request),
            cancellationToken).ConfigureAwait(false);

        return new TikTokPage<ProductSearchProductsResponseDataProducts>(
            response.Data.Products,
            response.Data.NextPageToken,
            response.Data.TotalCount);
    }

    public IAsyncEnumerable<ProductSearchProductsResponseDataProducts> SearchAllProductsAsync(
        TikTokAuthorizationContext context,
        TikTokProductSearchRequest request,
        CancellationToken cancellationToken = default)
        => TikTokPagedEnumerable.ReadAllAsync(
            new TikTokPageRequest(request.PageSize, request.PageToken),
            (page, token) => SearchProductsAsync(
                context,
                request with { PageSize = page.PageSize, PageToken = page.PageToken },
                token),
            cancellationToken);

    private static ProductSearchProductsRequest ToGeneratedRequest(
        TikTokAuthorizationContext context,
        TikTokProductSearchRequest request)
        => new(
            context.AppKey,
            0,
            string.Empty,
            request.PageSize,
            request.PageToken,
            TikTokManagerContext.RequireShopCipher(context),
            request.Status,
            request.SellerSkus ?? Array.Empty<string>(),
            request.CreateTimeGe,
            request.CreateTimeLe,
            request.UpdateTimeGe,
            request.UpdateTimeLe,
            request.CategoryVersion,
            request.ListingQualityTiers ?? Array.Empty<string>(),
            request.ListingPlatforms ?? Array.Empty<string>(),
            request.AuditStatus ?? Array.Empty<string>(),
            request.SkuIds ?? Array.Empty<string>(),
            request.SnsFilter,
            request.ReturnDraftVersion);
}
