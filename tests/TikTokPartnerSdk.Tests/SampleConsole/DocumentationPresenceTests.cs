using FluentAssertions;

namespace TikTokPartnerSdk.Tests.SampleConsole;

public sealed class DocumentationPresenceTests
{
    [Fact]
    public void Repository_should_include_readme_and_getting_started_docs()
    {
        var root = TestPaths.RepositoryRoot;

        File.Exists(Path.Combine(root, "README.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "docs", "getting-started.md")).Should().BeTrue();
    }
}
