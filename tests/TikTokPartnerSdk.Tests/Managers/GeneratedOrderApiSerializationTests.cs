using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Core.Crypto;
using Loscardos.TikTokPartnerSdk.Core.Http;
using Loscardos.TikTokPartnerSdk.Core.Managers.Generated;
using Loscardos.TikTokPartnerSdk.Core.RateLimiting;
using Loscardos.TikTokPartnerSdk.Generated.Order;
using Microsoft.Extensions.Options;

namespace Loscardos.TikTokPartnerSdk.Tests.Managers;

public sealed class GeneratedOrderApiSerializationTests
{
    [Fact]
    public async Task GetOrderList_should_omit_unused_optional_defaults_from_serialized_body()
    {
        var handler = new RecordingHandler();
        var partnerClient = new TikTokPartnerClient(
            new HttpClient(handler),
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
        var api = new OrderApi(partnerClient);

        await api.GetOrderListAsync(
            "access-token",
            new OrderGetOrderListRequest(
                "app-key",
                0,
                string.Empty,
                50,
                string.Empty,
                "shop-cipher",
                "update_time",
                "ASC",
                null!,
                null!,
                null!,
                1_700_000_000,
                1_700_003_600,
                null!,
                null!,
                null,
                null!),
            CancellationToken.None);

        using var body = JsonDocument.Parse(handler.RequestBody);
        var root = body.RootElement;

        root.GetProperty("update_time_ge").GetInt64().Should().Be(1_700_000_000);
        root.GetProperty("update_time_lt").GetInt64().Should().Be(1_700_003_600);
        root.TryGetProperty("create_time_ge", out _).Should().BeFalse();
        root.TryGetProperty("create_time_lt", out _).Should().BeFalse();
        root.TryGetProperty("order_status", out _).Should().BeFalse();
        root.TryGetProperty("shipping_type", out _).Should().BeFalse();
        root.TryGetProperty("buyer_user_id", out _).Should().BeFalse();
        root.TryGetProperty("is_buyer_request_cancel", out _).Should().BeFalse();
        root.TryGetProperty("warehouse_ids", out _).Should().BeFalse();
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public string RequestBody { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestBody = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """{"code":0,"message":"success","request_id":"req-1","data":{"next_page_token":"","total_count":0,"orders":[]}}""",
                    Encoding.UTF8,
                    "application/json")
            };
        }
    }
}
