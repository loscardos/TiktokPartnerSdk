using FluentAssertions;
using TikTokPartnerSdk.Generator;

namespace TikTokPartnerSdk.Tests.Generator;

public sealed class SchemaSnapshotReaderTests
{
    [Fact]
    public void Reader_should_load_normalized_snapshot_directory()
    {
        var reader = new SchemaReader();
        var directory = Path.Combine(
            TestPaths.RepositoryRoot,
            "tests",
            "TikTokPartnerSdk.Tests",
            "Fixtures",
            "Schemas");

        var endpoints = reader.ReadDirectory(directory);

        endpoints.Should().NotBeEmpty();
        endpoints.Should().Contain(x => x.Path == "/authorization/202309/shops");
        endpoints.Should().Contain(x => x.ModuleKey == "authorization");
    }

    [Fact]
    public void Reader_should_parse_request_and_response_parameters()
    {
        var reader = new SchemaReader();
        var directory = Path.Combine(
            TestPaths.RepositoryRoot,
            "tests",
            "TikTokPartnerSdk.Tests",
            "Fixtures",
            "Schemas");

        var endpoint = reader.ReadDirectory(directory)
            .Single(x => x.Path == "/authorization/202309/shops");

        endpoint.RequestParameters.Should().Contain(x => x.Name == "app_key" && x.Required);
        endpoint.ResponseParameters.Should().Contain(x => x.Name == "request_id");
    }

    [Fact]
    public void Reader_should_parse_nested_response_children()
    {
        var reader = new SchemaReader();
        var directory = Path.Combine(
            TestPaths.RepositoryRoot,
            "tests",
            "TikTokPartnerSdk.Tests",
            "Fixtures",
            "Schemas");

        var endpoint = reader.ReadDirectory(directory)
            .Single(x => x.Path == "/authorization/202405/category_assets");

        var data = endpoint.ResponseParameters.Single(x => x.Name == "data");
        var categoryAssets = data.Children.Single(x => x.Name == "category_assets");

        categoryAssets.Children.Should().Contain(x => x.Name == "cipher");
    }
}
