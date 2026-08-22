using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated;
using Loscardos.TikTokPartnerSdk.Abstractions.Pagination;
using Loscardos.TikTokPartnerSdk.Core.Auth;
using Loscardos.TikTokPartnerSdk.Generated.Product;

namespace Loscardos.TikTokPartnerSdk.Core.Managers;

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
            response.Data.Products ?? Array.Empty<ProductSearchProductsResponseDataProducts>(),
            response.Data.NextPageToken ?? string.Empty,
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
            Optional(request.Status)!,
            request.SellerSkus!,
            Optional(request.CreateTimeGe),
            Optional(request.CreateTimeLe),
            Optional(request.UpdateTimeGe),
            Optional(request.UpdateTimeLe),
            Optional(request.CategoryVersion)!,
            request.ListingQualityTiers!,
            request.ListingPlatforms!,
            request.AuditStatus!,
            request.SkuIds!,
            Optional(request.SnsFilter)!,
            request.ReturnDraftVersion ? true : null);

    private static string? Optional(string value)
        => string.IsNullOrWhiteSpace(value) ? null : value;

    private static long? Optional(long value)
        => value == 0 ? null : value;
}
