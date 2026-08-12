using FluentAssertions;
using Loscardos.TikTokPartnerSdk.Generator;

namespace Loscardos.TikTokPartnerSdk.Tests.Generator;

public sealed class YamlApiReferenceReaderTests
{
    [Fact]
    public void Reader_should_discover_seller_paths_from_yaml_reference()
    {
        var path = Path.Combine(TestPaths.RepositoryRoot, "tests", "TikTokPartnerSdk.Tests", "Fixtures", "YamlDocs", "seller.yaml");
        var endpoints = new YamlApiReferenceReader().ReadFile(path);

        endpoints.Select(static x => x.Path).Should().Contain("/seller/202309/shops");
        endpoints.Select(static x => x.Path).Should().Contain("/seller/202309/permissions");
        endpoints.Select(static x => x.Path).Should().Contain("/seller/202601/shop_groups");
    }

    [Fact]
    public void Reader_should_preserve_same_path_with_different_event_methods()
    {
        var path = Path.Combine(TestPaths.RepositoryRoot, "tests", "TikTokPartnerSdk.Tests", "Fixtures", "YamlDocs", "event.yaml");
        var endpoints = new YamlApiReferenceReader().ReadFile(path);

        endpoints.Where(static x => x.Path == "/event/202309/webhooks")
            .Select(static x => x.Method)
            .Should()
            .BeEquivalentTo(["DELETE", "GET", "PUT"]);
    }
}
