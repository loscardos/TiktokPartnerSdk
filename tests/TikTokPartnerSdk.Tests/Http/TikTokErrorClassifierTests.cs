using FluentAssertions;
using System.Net;
using Loscardos.TikTokPartnerSdk.Abstractions.Errors;
using Loscardos.TikTokPartnerSdk.Core.Http;

namespace Loscardos.TikTokPartnerSdk.Tests.Http;

public sealed class TikTokErrorClassifierTests
{
    [Theory]
    [InlineData(105005, "Invalid access token", TikTokErrorCategory.Authentication)]
    [InlineData(0, "Permission denied for shop scope", TikTokErrorCategory.Authorization)]
    [InlineData(0, "rate limit exceeded", TikTokErrorCategory.RateLimit)]
    [InlineData(500001, "System Error", TikTokErrorCategory.Transient)]
    [InlineData(400001, "invalid request parameter", TikTokErrorCategory.Validation)]
    public void Classify_should_map_known_api_error_categories(long code, string message, TikTokErrorCategory expected)
    {
        TikTokErrorClassifier.Classify(code, message).Should().Be(expected);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, TikTokErrorCategory.Authentication)]
    [InlineData(HttpStatusCode.Forbidden, TikTokErrorCategory.Authorization)]
    [InlineData(HttpStatusCode.TooManyRequests, TikTokErrorCategory.RateLimit)]
    [InlineData(HttpStatusCode.BadGateway, TikTokErrorCategory.Transient)]
    [InlineData(HttpStatusCode.BadRequest, TikTokErrorCategory.Validation)]
    public void Classify_should_map_http_error_categories(HttpStatusCode statusCode, TikTokErrorCategory expected)
    {
        TikTokErrorClassifier.Classify(statusCode).Should().Be(expected);
    }
}
