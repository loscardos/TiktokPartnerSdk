using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Structure;

public sealed class SolutionStructureTests
{
    [Fact]
    public void Solution_should_contain_expected_projects()
    {
        var root = TestPaths.RepositoryRoot;

        File.Exists(Path.Combine(root, "TikTokPartnerSdk.sln")).Should().BeTrue();
        Directory.Exists(Path.Combine(root, "src", "TikTokPartnerSdk.Abstractions")).Should().BeTrue();
        Directory.Exists(Path.Combine(root, "src", "TikTokPartnerSdk.Core")).Should().BeTrue();
        Directory.Exists(Path.Combine(root, "src", "TikTokPartnerSdk.Generated")).Should().BeTrue();
        Directory.Exists(Path.Combine(root, "src", "TikTokPartnerSdk.Extensions.DependencyInjection")).Should().BeTrue();
        Directory.Exists(Path.Combine(root, "src", "TikTokPartnerSdk.Generator")).Should().BeTrue();
    }
}
