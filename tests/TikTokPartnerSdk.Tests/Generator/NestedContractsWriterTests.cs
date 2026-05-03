using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Generator;

public sealed class NestedContractsWriterTests
{
    [Fact]
    public void Generated_authorization_contract_should_include_nested_category_asset_types()
    {
        var path = Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Generated",
            "Authorization",
            "AuthorizationContracts.g.cs");

        var contents = File.ReadAllText(path);

        contents.Should().Contain("AuthorizationGetAuthorizedCategoryAssetsResponseData");
        contents.Should().Contain("IReadOnlyList<AuthorizationGetAuthorizedCategoryAssetsResponseDataCategoryAssets>");
        contents.Should().Contain("AuthorizationGetAuthorizedCategoryAssetsResponseDataCategoryAssetsCategory");
    }
}
