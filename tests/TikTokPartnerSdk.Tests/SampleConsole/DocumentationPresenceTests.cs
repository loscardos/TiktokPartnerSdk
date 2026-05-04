using FluentAssertions;

namespace TikTokPartnerSdk.Tests.SampleConsole;

public sealed class DocumentationPresenceTests
{
    [Fact]
    public void Readme_should_link_core_production_docs()
    {
        var readme = File.ReadAllText(Path.Combine(TestPaths.RepositoryRoot, "README.md"));

        readme.Should().Contain("docs/getting-started.md");
        readme.Should().Contain("docs/auth.md");
        readme.Should().Contain("docs/sandbox.md");
        readme.Should().Contain("docs/storage.md");
        readme.Should().Contain("docs/packaging.md");
    }

    [Fact]
    public void Getting_started_and_auth_docs_should_include_required_snippets()
    {
        var root = TestPaths.RepositoryRoot;
        var gettingStarted = File.ReadAllText(Path.Combine(root, "docs", "getting-started.md"));
        var auth = File.ReadAllText(Path.Combine(root, "docs", "auth.md"));

        gettingStarted.Should().Contain("AddTikTokPartnerSdk");
        gettingStarted.Should().Contain("IAuthApi");
        auth.Should().Contain("BuildAuthorizationUrl");
        auth.Should().Contain("ExchangeCodeAsync");
    }

    [Fact]
    public void Production_docs_should_exist_for_runtime_storage_and_packaging()
    {
        var root = TestPaths.RepositoryRoot;

        File.Exists(Path.Combine(root, "docs", "sandbox.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "docs", "storage.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "docs", "runtime.md")).Should().BeTrue();
        File.Exists(Path.Combine(root, "docs", "packaging.md")).Should().BeTrue();
    }
}
