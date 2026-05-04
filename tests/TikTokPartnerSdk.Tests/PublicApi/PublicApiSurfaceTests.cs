using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Pagination;
using TikTokPartnerSdk.Abstractions.Webhooks;

namespace TikTokPartnerSdk.Tests.PublicApi;

public sealed class PublicApiSurfaceTests
{
    [Fact]
    public void Auth_api_contract_should_be_available()
    {
        typeof(IAuthApi).Should().NotBeNull();
    }

    [Fact]
    public void Polished_manager_contracts_should_be_available()
    {
        typeof(IOrderManager).Should().NotBeNull();
        typeof(IProductManager).Should().NotBeNull();
        typeof(TikTokPage<>).Should().NotBeNull();
        typeof(TikTokPagedEnumerable).Should().NotBeNull();
    }

    [Fact]
    public void Webhook_contracts_should_be_available()
    {
        typeof(TikTokWebhookEvent).Should().NotBeNull();
        typeof(TikTokWebhookEnvelope).Should().NotBeNull();
        typeof(TikTokWebhookReceiveResult).Should().NotBeNull();
        typeof(TikTokWebhookTypedEvent<>).Should().NotBeNull();
        typeof(ITikTokWebhookSignatureVerifier).Should().NotBeNull();
        typeof(ITikTokWebhookIdempotencyKeyFactory).Should().NotBeNull();
        typeof(ITikTokWebhookParser).Should().NotBeNull();
        typeof(ITikTokWebhookRawPayloadSink).Should().NotBeNull();
        typeof(ITikTokWebhookQueue).Should().NotBeNull();
    }
}
