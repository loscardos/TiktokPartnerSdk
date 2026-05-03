using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Managers;

namespace TikTokPartnerSdk.Tests.PublicApi;

public sealed class PublicApiSurfaceTests
{
    [Fact]
    public void Auth_api_contract_should_be_available()
    {
        typeof(IAuthApi).Should().NotBeNull();
    }
}
