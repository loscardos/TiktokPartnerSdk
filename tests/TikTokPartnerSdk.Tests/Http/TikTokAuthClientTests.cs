using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Core.Http;
using TikTokPartnerSdk.Core.RateLimiting;

namespace TikTokPartnerSdk.Tests.Http;

public sealed class TikTokAuthClientTests
{
    [Fact]
    public async Task GetAsync_should_send_token_query_to_auth_base_url_without_open_api_signature()
    {
        var handler = new RecordingHandler("""{"code":0,"message":"success","requestId":"req-auth","data":{"accessToken":"access","refreshToken":"refresh","accessTokenExpireIn":7200,"refreshTokenExpireIn":2592000}}""");
        var client = new TikTokAuthClient(
            new HttpClient(handler),
            Options.Create(new TikTokPartnerOptions { AuthApiBaseUrl = "https://auth.example.test/api/v2" }),
            new NoopTikTokRateLimiter(),
            new TikTokResponseParser());

        var envelope = await client.GetAsync<AuthPayload>(
            "/token/get",
            new Dictionary<string, object?>
            {
                ["app_key"] = "app-key",
                ["app_secret"] = "app-secret",
                ["auth_code"] = "code-1",
                ["grant_type"] = "authorized_code"
            },
            CancellationToken.None);

        envelope.Code.Should().Be(0);
        envelope.RequestId.Should().Be("req-auth");
        envelope.Data.Should().NotBeNull();
        envelope.Data!.AccessToken.Should().Be("access");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Get);
        handler.LastRequest.RequestUri!.ToString().Should().Contain("https://auth.example.test/api/v2/token/get?");
        handler.LastRequest.RequestUri.Query.Should().Contain("auth_code=code-1");
        handler.LastRequest.RequestUri.Query.Should().Contain("grant_type=authorized_code");
        handler.LastRequest.Headers.UserAgent.ToString().Should().Be("InternalTikTokPartnerSdk/0.1");
        handler.LastRequestBody.Should().BeEmpty();
        handler.LastRequestBody.Should().NotContain("sign");
    }

    private sealed record AuthPayload(
        string AccessToken,
        string RefreshToken,
        long AccessTokenExpireIn,
        long RefreshTokenExpireIn);

    private sealed class RecordingHandler(string response) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string LastRequestBody { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            LastRequestBody = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
        }
    }
}
