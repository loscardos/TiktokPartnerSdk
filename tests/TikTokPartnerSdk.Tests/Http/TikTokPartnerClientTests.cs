using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Core.Crypto;
using TikTokPartnerSdk.Core.Http;

namespace TikTokPartnerSdk.Tests.Http;

public sealed class TikTokPartnerClientTests
{
    [Fact]
    public async Task SendAsync_should_append_app_key_timestamp_sign_and_access_token_header()
    {
        var handler = new RecordingHandler("""{"code":0,"message":"success","request_id":"req-1","data":{"ok":true}}""");
        var httpClient = new HttpClient(handler);
        var client = new TikTokPartnerClient(
            httpClient,
            Options.Create(new TikTokPartnerOptions
            {
                AppKey = "app-key",
                AppSecret = "app-secret"
            }),
            new TikTokRequestSigner(),
            new TikTokRequestUriBuilder(),
            new TikTokRequestContentFactory(),
            new TikTokResponseParser());

        await client.SendAsync<Dictionary<string, bool>>(
            new TikTokPartnerRequest(
                HttpMethod.Get,
                "/authorization/202309/shops",
                new Dictionary<string, object?>(),
                null,
                new TikTokAuthorizationContext(TikTokAccessTokenKind.Seller, "app-key", "cipher-1"),
                "token-1"),
            CancellationToken.None);

        handler.LastRequest.Should().NotBeNull();
        handler.LastRequest!.RequestUri!.Query.Should().Contain("app_key=app-key");
        handler.LastRequest.RequestUri.Query.Should().Contain("timestamp=");
        handler.LastRequest.RequestUri.Query.Should().Contain("sign=");
        handler.LastRequest.Headers.TryGetValues("x-tts-access-token", out var values).Should().BeTrue();
        values!.Single().Should().Be("token-1");
    }

    private sealed class RecordingHandler(string responseBody) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });
        }
    }
}
