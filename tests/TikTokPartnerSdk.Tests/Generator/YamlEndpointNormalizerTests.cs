using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Generator;

namespace Loscardos.TikTokPartnerSdk.Tests.Generator;

public sealed class YamlEndpointNormalizerTests
{
    [Fact]
    public void Normalizer_should_map_seller_docs_to_schema_endpoint()
    {
        var path = Path.Combine(TestPaths.RepositoryRoot, "tests", "TikTokPartnerSdk.Tests", "Fixtures", "YamlDocs", "seller.yaml");
        var docsEndpoint = new YamlApiReferenceReader().ReadFile(path)
            .Single(static endpoint => endpoint.Path == "/seller/202309/permissions");

        var endpoint = new YamlEndpointNormalizer().Normalize(docsEndpoint);

        endpoint.ModuleName.Should().Be("Seller");
        endpoint.ModuleKey.Should().Be("seller");
        endpoint.OperationId.Should().Be("seller.202309.get_seller_permissions");
        endpoint.RequestContentKind.Should().Be("query");
        endpoint.RequestParameters.Should().Contain(parameter => parameter.Name == "app_key" && parameter.Location == "query");
        endpoint.ResponseParameters.Should().Contain(parameter => parameter.Name == "data");
    }

    [Fact]
    public void Normalizer_should_map_body_and_query_locations_for_event_docs()
    {
        var path = Path.Combine(TestPaths.RepositoryRoot, "tests", "TikTokPartnerSdk.Tests", "Fixtures", "YamlDocs", "event.yaml");
        var docsEndpoint = new YamlApiReferenceReader().ReadFile(path)
            .Single(static endpoint => endpoint.Method == "DELETE" && endpoint.Path == "/event/202309/webhooks");

        var endpoint = new YamlEndpointNormalizer().Normalize(docsEndpoint);

        endpoint.RequestContentKind.Should().Be("body");
        endpoint.RequestParameters.Should().Contain(parameter => parameter.Name == "shop_cipher" && parameter.Location == "query");
        endpoint.RequestParameters.Should().Contain(parameter => parameter.Name == "event_type" && parameter.Location == "body");
    }

    [Fact]
    public void Normalizer_should_build_nested_body_tree_for_product_create()
    {
        var path = Path.Combine(TestPaths.RepositoryRoot, "tests", "TikTokPartnerSdk.Tests", "Fixtures", "YamlDocs", "products.yaml");
        var docsEndpoint = new YamlApiReferenceReader().ReadFile(path)
            .Single(static endpoint => endpoint.Slug == "create-product-202309");

        var endpoint = new YamlEndpointNormalizer().Normalize(docsEndpoint);

        endpoint.RequestContentKind.Should().Be("body");
        endpoint.RequestParameters.Should().Contain(parameter => parameter.Name == "shop_cipher" && parameter.Location == "query");

        var title = endpoint.RequestParameters.Single(parameter => parameter.Name == "title");
        title.Location.Should().Be("body");

        var packageWeight = endpoint.RequestParameters.Single(parameter => parameter.Name == "package_weight");
        packageWeight.Location.Should().Be("body");
        packageWeight.Children.Should().Contain(parameter => parameter.Name == "value");
        packageWeight.Children.Should().Contain(parameter => parameter.Name == "unit");
    }
}
