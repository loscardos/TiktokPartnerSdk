using FluentAssertions;
using TikTokPartnerSdk.Core.Http;

namespace TikTokPartnerSdk.Tests.Http;

public sealed class TikTokRequestContentFactoryTests
{
    [Fact]
    public async Task Create_should_omit_null_dictionary_values()
    {
        var factory = new TikTokRequestContentFactory();

        using var content = factory.Create(new Dictionary<string, object?>
        {
            ["status"] = null,
            ["page_size"] = 10
        });

        content.Should().NotBeNull();
        var json = await content!.ReadAsStringAsync();
        json.Should().Be("""{"page_size":10}""");
    }
}
