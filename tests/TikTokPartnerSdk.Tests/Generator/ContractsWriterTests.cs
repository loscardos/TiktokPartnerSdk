using FluentAssertions;
using TikTokPartnerSdk.Generator;

namespace TikTokPartnerSdk.Tests.Generator;

public sealed class ContractsWriterTests
{
    [Fact]
    public void Generated_authorization_contract_should_include_request_properties()
    {
        var path = Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Generated",
            "Authorization",
            "AuthorizationContracts.g.cs");

        File.Exists(path).Should().BeTrue();
        var contents = File.ReadAllText(path);

        contents.Should().Contain("AuthorizationGetAuthorizedShopsRequest");
        contents.Should().Contain("string AppKey");
        contents.Should().Contain("long Timestamp");
    }

    [Fact]
    public void Request_contract_should_disambiguate_duplicate_parameter_names_by_location()
    {
        var endpoint = new SchemaEndpoint(
            "promotion.202309.update_activity_product",
            "Promotion",
            "promotion",
            "/promotion/202309/activities/{activity_id}/products",
            "PUT",
            "seller",
            "seller",
            "body",
            ["x-tts-access-token", "content-type"],
            [
                new SchemaParameter("activity_id", "string", true, "path", []),
                new SchemaParameter("app_key", "string", true, "query", []),
                new SchemaParameter("timestamp", "int", true, "query", []),
                new SchemaParameter("sign", "string", true, "query", []),
                new SchemaParameter("shop_cipher", "string", true, "query", []),
                new SchemaParameter("activity_id", "string", true, "body", [])
            ],
            [
                new SchemaParameter("code", "int", false, "body", []),
                new SchemaParameter("message", "string", false, "body", []),
                new SchemaParameter("request_id", "string", false, "body", [])
            ]);

        var output = new ContractsWriter().WriteModule("Promotion", "promotion", [endpoint]);

        output.Should().Contain("[property: JsonPropertyName(\"activity_id\")] string PathActivityId,");
        output.Should().Contain("[property: JsonPropertyName(\"activity_id\")] string BodyActivityId");
        output.Should().NotContain("string ActivityId,");
    }

    [Fact]
    public void Response_contract_should_emit_valid_identifier_for_numeric_property_name()
    {
        var endpoint = new SchemaEndpoint(
            "analytics.202509.get_shop_live_performance_list",
            "Analytics",
            "analytics",
            "/analytics/202509/live/performance",
            "GET",
            "seller",
            "seller",
            "query",
            ["x-tts-access-token", "content-type"],
            [
                new SchemaParameter("app_key", "string", true, "query", []),
                new SchemaParameter("timestamp", "int", true, "query", []),
                new SchemaParameter("sign", "string", true, "query", [])
            ],
            [
                new SchemaParameter("data", "object", false, "body", [
                    new SchemaParameter("24h_live_gmv", "object", false, "body", [
                        new SchemaParameter("amount", "string", false, "body", [])
                    ])
                ])
            ]);

        var output = new ContractsWriter().WriteModule("Analytics", "analytics", [endpoint]);

        output.Should().Contain("[property: JsonPropertyName(\"24h_live_gmv\")] AnalyticsGetShopLivePerformanceListResponseDataValue24hLiveGmv Value24hLiveGmv");
        output.Should().NotContain(" 24hLiveGmv");
    }

    [Fact]
    public void Optional_contract_members_should_omit_default_values()
    {
        var endpoint = new SchemaEndpoint(
            "product.202309.update_inventory",
            "Product",
            "product",
            "/product/202309/products/{product_id}/inventory/update",
            "POST",
            "seller",
            "seller",
            "body",
            ["x-tts-access-token", "content-type"],
            [
                new SchemaParameter("quantity", "int", true, "body", []),
                new SchemaParameter("backorder_quantity", "int", false, "body", [])
            ],
            []);

        var output = new ContractsWriter().WriteModule(
            "Product",
            "product",
            [endpoint]);

        output.Should().Contain(
            "[property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]");
        output.Should().Contain(
            "[property: JsonPropertyName(\"backorder_quantity\")] long BackorderQuantity");
        output.Should().NotContain(
            "JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]\n    [property: JsonPropertyName(\"quantity\")]");
    }
}
