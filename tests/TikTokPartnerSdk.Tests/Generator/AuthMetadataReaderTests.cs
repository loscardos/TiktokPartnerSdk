using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Generator;

namespace Loscardos.TikTokPartnerSdk.Tests.Generator;

public sealed class AuthMetadataReaderTests
{
    [Fact]
    public void Reader_should_parse_access_token_kind_and_header_requirements()
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

        endpoint.AccessTokenKind.Should().Be("partner");
        endpoint.RequiredHeaders.Should().Contain("x-tts-access-token");
    }
}
