using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Generator;

namespace Loscardos.TikTokPartnerSdk.Tests.Generator;

public sealed class ManagersWriterTests
{
    [Fact]
    public void WriteImplementation_should_emit_runtime_client_call_for_query_endpoint()
    {
        var endpoint = new SchemaEndpoint(
            "authorization.202309.get_authorized_shops",
            "Authorization",
            "authorization",
            "/authorization/202309/shops",
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
                new SchemaParameter("code", "int", false, "body", []),
                new SchemaParameter("message", "string", false, "body", []),
                new SchemaParameter("request_id", "string", false, "body", [])
            ]);

        var output = new ManagersWriter().WriteImplementation("Authorization", "authorization", [endpoint]);

        output.Should().Contain("var envelope = await client.SendAsync<object>(");
        output.Should().Contain("HttpMethod.Get");
        output.Should().Contain("\"/authorization/202309/shops\"");
        output.Should().Contain("null,");
        output.Should().Contain("accessToken),");
        output.Should().NotContain("throw new NotImplementedException();");
    }

    [Fact]
    public void Implementation_should_replace_path_parameters_from_request()
    {
        var endpoint = new SchemaEndpoint(
            "order.202406.get_external_order_references",
            "Order",
            "order",
            "/order/202406/orders/{order_id}/external_orders",
            "GET",
            "seller",
            "seller",
            "query",
            ["x-tts-access-token", "content-type"],
            [
                new SchemaParameter("app_key", "string", true, "query", []),
                new SchemaParameter("timestamp", "int", true, "query", []),
                new SchemaParameter("sign", "string", true, "query", []),
                new SchemaParameter("order_id", "string", true, "path", [])
            ],
            [
                new SchemaParameter("code", "int", false, "body", []),
                new SchemaParameter("message", "string", false, "body", []),
                new SchemaParameter("request_id", "string", false, "body", [])
            ]);

        var output = new ManagersWriter().WriteImplementation("Order", "order", [endpoint]);

        output.Should().Contain("var path = \"/order/202406/orders/{order_id}/external_orders\";");
        output.Should().Contain("path = path.Replace(\"{order_id}\", Uri.EscapeDataString(Convert.ToString(request.OrderId, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty));");
        output.Should().Contain("path,");
        output.Should().NotContain("query[\"order_id\"]");
    }

    [Fact]
    public void Implementation_should_use_disambiguated_duplicate_request_parameter_names()
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

        var output = new ManagersWriter().WriteImplementation("Promotion", "promotion", [endpoint]);

        output.Should().Contain("body[\"activity_id\"] = request.BodyActivityId;");
        output.Should().Contain("path = path.Replace(\"{activity_id}\", Uri.EscapeDataString(Convert.ToString(request.PathActivityId, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty));");
    }

    [Fact]
    public void Implementation_should_add_required_parameters_and_filter_optional_parameters_generically()
    {
        var endpoint = new SchemaEndpoint(
            "order.202309.get_order_list",
            "Order",
            "order",
            "/order/202309/orders/search",
            "POST",
            "seller",
            "seller",
            "body",
            ["x-tts-access-token", "content-type"],
            [
                new SchemaParameter("page_size", "int", true, "query", []),
                new SchemaParameter("page_token", "string", false, "query", []),
                new SchemaParameter("update_time_ge", "int", false, "body", []),
                new SchemaParameter("warehouse_ids", "[]string", false, "body", []),
                new SchemaParameter("orders", "[]object", true, "body", [])
            ],
            [
                new SchemaParameter("code", "int", false, "body", []),
                new SchemaParameter("message", "string", false, "body", []),
                new SchemaParameter("request_id", "string", false, "body", [])
            ]);

        var output = new ManagersWriter().WriteImplementation("Order", "order", [endpoint]);

        output.Should().Contain("query[\"page_size\"] = request.PageSize;");
        output.Should().Contain("TikTokGeneratedRequestMap.AddOptional(query, \"page_token\", request.PageToken);");
        output.Should().Contain("TikTokGeneratedRequestMap.AddOptional(body, \"update_time_ge\", request.UpdateTimeGe);");
        output.Should().Contain("TikTokGeneratedRequestMap.AddOptional(body, \"warehouse_ids\", request.WarehouseIds);");
        output.Should().Contain("body[\"orders\"] = request.Orders;");
    }
}
