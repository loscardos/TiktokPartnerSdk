using FluentAssertions;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Pagination;

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
}
