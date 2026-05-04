using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Errors;
using TikTokPartnerSdk.Core.Http;

namespace TikTokPartnerSdk.Tests.Http;

public sealed class TikTokResponseParserTests
{
    [Fact]
    public void Parse_should_throw_typed_exception_for_tiktok_error()
    {
        var parser = new TikTokResponseParser();

        var action = () => parser.Parse<Dictionary<string, bool>>("""{"code":105005,"message":"Invalid access token","request_id":"req-error","data":null}""");

        var exception = action.Should().Throw<TikTokApiException>().Which;
        exception.Code.Should().Be(105005);
        exception.RequestId.Should().Be("req-error");
        exception.Category.Should().Be(TikTokErrorCategory.Authentication);
    }
}
