using FluentAssertions;
using TikTokPartnerSdk.Core.Http;

namespace TikTokPartnerSdk.Tests.Http;

public sealed class TikTokResponseParserTests
{
    [Fact]
    public void Parse_should_throw_when_tiktok_business_code_is_non_zero()
    {
        var parser = new TikTokResponseParser();

        var action = () => parser.Parse<Dictionary<string, bool>>("""{"code":106001,"message":"bad request","request_id":"req-1","data":null}""");

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*106001*");
    }
}
