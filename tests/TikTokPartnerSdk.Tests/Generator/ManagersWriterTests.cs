using FluentAssertions;
using TikTokPartnerSdk.Generator;

namespace TikTokPartnerSdk.Tests.Generator;

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
}
