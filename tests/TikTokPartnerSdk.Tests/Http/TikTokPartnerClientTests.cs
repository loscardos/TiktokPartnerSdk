using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.Http;
using Loscardos.TikTokPartnerSdk.Core.RateLimiting;
using Loscardos.TikTokPartnerSdk.Core.Crypto;
using Loscardos.TikTokPartnerSdk.Core.Http;

namespace Loscardos.TikTokPartnerSdk.Tests.Http;

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
            new NoopTikTokRateLimiter(),
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

    [Fact]
    public async Task SendAsync_should_serialize_query_lists_as_comma_separated_values()
    {
        var handler = new RecordingHandler("""{"code":0,"message":"success","request_id":"req-1","data":{"ok":true}}""");
        var client = CreatePartnerClient(handler);

        await client.SendAsync<Dictionary<string, bool>>(
            new TikTokPartnerRequest(
                HttpMethod.Get,
                "/finance/202309/withdrawals",
                new Dictionary<string, object?>
                {
                    ["types"] = new[] { "WITHDRAW", "SETTLE" }
                },
                null,
                null,
                "token-1"),
            CancellationToken.None);

        handler.LastRequest!.RequestUri!.Query.Should().Contain("types=WITHDRAW%2CSETTLE");
        handler.LastRequest.RequestUri.Query.Should().NotContain("%5B");
    }


    [Fact]
    public async Task SendAsync_should_retry_transient_http_status()
    {
        var handler = new SequenceHandler(
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("""{"code":500001,"message":"System Error","request_id":"req-1"}""", Encoding.UTF8, "application/json")
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"code":0,"message":"success","request_id":"req-2","data":{"ok":true}}""", Encoding.UTF8, "application/json")
            });

        var client = CreatePartnerClient(handler, options =>
        {
            options.MaxTransientRetries = 1;
            options.RetryBaseDelay = TimeSpan.Zero;
        });

        await client.SendAsync<Dictionary<string, bool>>(CreateRequest(), CancellationToken.None);

        handler.RequestCount.Should().Be(2);
    }

    private static TikTokPartnerClient CreatePartnerClient(
        HttpMessageHandler handler,
        Action<TikTokPartnerOptions>? configure = null)
    {
        var options = new TikTokPartnerOptions
        {
            AppKey = "app-key",
            AppSecret = "app-secret"
        };
        configure?.Invoke(options);

        return new TikTokPartnerClient(
            new HttpClient(handler),
            Options.Create(options),
            new TikTokRequestSigner(),
            new TikTokRequestUriBuilder(),
            new TikTokRequestContentFactory(),
            new NoopTikTokRateLimiter(),
            new TikTokResponseParser());
    }

    private static TikTokPartnerRequest CreateRequest()
        => new(
            HttpMethod.Get,
            "/authorization/202309/shops",
            new Dictionary<string, object?>(),
            null,
            new TikTokAuthorizationContext(TikTokAccessTokenKind.Seller, "app-key", "cipher-1"),
            "token-1");

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

    private sealed class SequenceHandler(params HttpResponseMessage[] responses) : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new(responses);

        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            return Task.FromResult(_responses.Dequeue());
        }
    }
}
