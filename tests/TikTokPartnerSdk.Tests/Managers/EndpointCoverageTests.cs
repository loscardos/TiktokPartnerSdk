using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Managers.Generated;

namespace TikTokPartnerSdk.Tests.Managers;

public sealed class EndpointCoverageTests
{
    [Fact]
    public void Starter_generated_interfaces_should_cover_authorization_seller_and_event_categories()
    {
        typeof(IAuthorizationApi).Should().NotBeNull();
        typeof(ISellerApi).Should().NotBeNull();
        typeof(IEventApi).Should().NotBeNull();
    }
}
