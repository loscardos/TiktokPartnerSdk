using FluentAssertions;
using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.Http;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;
using Loscardos.TikTokPartnerSdk.Core.Auth;
using Loscardos.TikTokPartnerSdk.Core.Managers;
using Loscardos.TikTokPartnerSdk.Core.Managers.Generated;
using Loscardos.TikTokPartnerSdk.Generated.Product;

namespace Loscardos.TikTokPartnerSdk.Tests.Managers;

public sealed class ProductManagerTests
{
    [Fact]
    public async Task GetProductAsync_should_resolve_token_and_call_generated_api()
    {
        var client = new RecordingClient();
        var manager = CreateManager(client);

        await manager.GetProductAsync(
            CreateContext(),
            "product-1",
            new TikTokProductDetailOptions(Locale: "en", ReturnDraftVersion: true),
            CancellationToken.None);

        client.LastRequest.Should().NotBeNull();
        client.LastRequest!.AccessToken.Should().Be("seller-access");
        client.LastRequest.Path.Should().Be("/product/202309/products/product-1");
        client.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        client.LastRequest.Query.Should().ContainKey("locale").WhoseValue.Should().Be("en");
        client.LastRequest.Query.Should().ContainKey("return_draft_version").WhoseValue.Should().Be(true);
    }

    [Fact]
    public async Task SearchProductsAsync_should_map_empty_optional_filters_to_null()
    {
        var client = new RecordingClient(
            new TikTokPartnerResponseEnvelope<ProductSearchProductsResponseData>(
                0,
                "success",
                "req-1",
                new ProductSearchProductsResponseData(0, [], string.Empty)));
        var manager = CreateManager(client);

        await manager.SearchProductsAsync(
            CreateContext(),
            new TikTokProductSearchRequest(PageSize: 10),
            CancellationToken.None);

        var body = client.LastRequest!.Body.Should().BeAssignableTo<IReadOnlyDictionary<string, object?>>().Subject;
        body.Should().NotContainKey("status");
        body.Should().NotContainKey("category_version");
        body.Should().NotContainKey("sns_filter");
    }

    [Fact]
    public async Task SearchProductsAsync_should_treat_missing_items_as_empty_page()
    {
        var client = new RecordingClient(
            new TikTokPartnerResponseEnvelope<ProductSearchProductsResponseData>(
                0,
                "success",
                "req-1",
                new ProductSearchProductsResponseData(0, null!, null!)));
        var manager = CreateManager(client);

        var page = await manager.SearchProductsAsync(
            CreateContext(),
            new TikTokProductSearchRequest(PageSize: 10),
            CancellationToken.None);

        page.Items.Should().BeEmpty();
        page.NextPageToken.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchProductsAsync_should_return_page_from_generated_response()
    {
        var client = new RecordingClient(
            new TikTokPartnerResponseEnvelope<ProductSearchProductsResponseData>(
                0,
                "success",
                "req-1",
                new ProductSearchProductsResponseData(20, [], "next")));
        var manager = CreateManager(client);

        var page = await manager.SearchProductsAsync(
            CreateContext(),
            new TikTokProductSearchRequest(PageSize: 50, PageToken: "start", Status: "ACTIVATE"),
            CancellationToken.None);

        page.TotalCount.Should().Be(20);
        page.NextPageToken.Should().Be("next");
        client.LastRequest!.Method.Should().Be(HttpMethod.Post);
        client.LastRequest.Path.Should().Be("/product/202502/products/search");
        client.LastRequest.Query.Should().ContainKey("shop_cipher").WhoseValue.Should().Be("shop-cipher");
        client.LastRequest.Query.Should().ContainKey("page_size").WhoseValue.Should().Be(50);
        client.LastRequest.Query.Should().ContainKey("page_token").WhoseValue.Should().Be("start");
        client.LastRequest.Body.Should().BeEquivalentTo(new Dictionary<string, object?>
        {
            ["status"] = "ACTIVATE"
        });
    }

    private static ProductManager CreateManager(RecordingClient client)
    {
        var token = new TikTokTokenRecord(
            TikTokAccessTokenKind.Seller,
            "seller-access",
            "refresh",
            DateTimeOffset.UtcNow.AddHours(1),
            DateTimeOffset.UtcNow.AddDays(30),
            "shop-cipher",
            "app-key");

        return new ProductManager(
            new ProductApi(client),
            new TikTokTokenService(
                new InMemoryTokenStore(token),
                new StubAuthApi(token),
                Options.Create(new TikTokPartnerOptions())));
    }

    private static TikTokAuthorizationContext CreateContext()
        => new(TikTokAccessTokenKind.Seller, "app-key", "shop-cipher");

    private sealed class RecordingClient(object response) : ITikTokPartnerClient
    {
        public RecordingClient()
            : this(new object())
        {
        }

        public TikTokPartnerRequest? LastRequest { get; private set; }

        public Task<TikTokPartnerResponseEnvelope<TResponse>> SendAsync<TResponse>(
            TikTokPartnerRequest request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (response is TikTokPartnerResponseEnvelope<TResponse> envelope)
            {
                return Task.FromResult(envelope);
            }

            return Task.FromResult(new TikTokPartnerResponseEnvelope<TResponse>(
                0,
                "success",
                "req-generated",
                (TResponse)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(TResponse))));
        }
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
