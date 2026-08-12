using FluentAssertions;

namespace Loscardos.TikTokPartnerSdk.Tests.Structure;

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

    [Fact]
    public void Ci_workflow_should_run_build_tests_and_generated_verification()
    {
        var workflow = File.ReadAllText(Path.Combine(TestPaths.RepositoryRoot, ".github", "workflows", "ci.yml"));

        workflow.Should().Contain("dotnet restore");
        workflow.Should().Contain("dotnet build");
        workflow.Should().Contain("dotnet test");
        workflow.Should().Contain("scripts/verify-generated.sh");
    }

    [Fact]
    public void Sandbox_workflow_should_be_manual_and_set_sandbox_flag()
    {
        var workflow = File.ReadAllText(Path.Combine(TestPaths.RepositoryRoot, ".github", "workflows", "sandbox.yml"));

        workflow.Should().Contain("workflow_dispatch");
        workflow.Should().Contain("TIKTOK_RUN_SANDBOX: true");
        workflow.Should().Contain("TIKTOK_SANDBOX_APP_KEY");
        workflow.Should().Contain("TIKTOK_SANDBOX_REDIRECT_URL");
        workflow.Should().NotContain("TIKTOK_SANDBOX_REDIRECT_URI");
        workflow.Should().Contain("tests/TikTokPartnerSdk.IntegrationTests");
    }

    [Fact]
    public void Package_workflow_should_publish_preverified_packages_only_from_production()
    {
        var workflow = File.ReadAllText(Path.Combine(TestPaths.RepositoryRoot, ".github", "workflows", "package.yml"));

        workflow.Should().Contain("- production");
        workflow.Should().Contain("scripts/pack-local.sh");
        workflow.Should().Contain("nuget.pkg.github.com/loscardos/index.json");
        workflow.Should().Contain("packages: write");
        workflow.Should().NotContain("dotnet test");
        workflow.Should().NotContain("upload-artifact");
    }
}
