using FluentAssertions;
using TikTokPartnerSdk.Generator;

namespace TikTokPartnerSdk.Tests.Generator;

public sealed class SchemaReaderTests
{
    [Fact]
    public void Reader_should_load_authorization_endpoints_from_yaml_category_file()
    {
        var reader = new SchemaReader();
        var path = Path.Combine(TestPaths.RepositoryRoot, "docs", "api-reference-sdk", "authorization.yaml");

        var category = reader.ReadCategory(path);

        category.Name.Should().Be("Authorization");
        category.Endpoints.Should().Contain(x => x.Path == "/authorization/202309/shops");
    }
}
