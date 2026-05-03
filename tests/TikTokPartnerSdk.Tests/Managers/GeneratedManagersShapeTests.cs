using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Managers;

public sealed class GeneratedManagersShapeTests
{
    [Fact]
    public void Generated_manager_interfaces_should_exist_for_starter_categories()
    {
        var root = TestPaths.RepositoryRoot;

        File.Exists(Path.Combine(root, "src", "TikTokPartnerSdk.Abstractions", "Managers", "Generated", "IAuthorizationApi.g.cs")).Should().BeTrue();
        File.Exists(Path.Combine(root, "src", "TikTokPartnerSdk.Abstractions", "Managers", "Generated", "ISellerApi.g.cs")).Should().BeTrue();
        File.Exists(Path.Combine(root, "src", "TikTokPartnerSdk.Abstractions", "Managers", "Generated", "IEventApi.g.cs")).Should().BeTrue();
    }
}
