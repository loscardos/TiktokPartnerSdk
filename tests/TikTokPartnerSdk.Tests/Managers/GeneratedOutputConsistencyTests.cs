using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Managers;

public sealed class GeneratedOutputConsistencyTests
{
    [Fact]
    public void Generated_runtime_surface_should_exist_without_docs_schema_files()
    {
        File.Exists(Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Generated",
            "Authorization",
            "AuthorizationContracts.g.cs")).Should().BeTrue();

        File.Exists(Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Abstractions",
            "Managers",
            "Generated",
            "IAuthorizationApi.g.cs")).Should().BeTrue();
    }

    [Fact]
    public void Generated_runtime_surface_should_include_nested_authorization_types()
    {
        var contents = File.ReadAllText(Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Generated",
            "Authorization",
            "AuthorizationContracts.g.cs"));

        contents.Should().Contain("AuthorizationGetAuthorizedCategoryAssetsResponseData");
    }
}
