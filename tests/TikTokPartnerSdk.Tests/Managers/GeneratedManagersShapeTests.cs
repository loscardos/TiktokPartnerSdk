using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Managers;

public sealed class GeneratedManagersShapeTests
{
    [Fact]
    public void Authorization_manager_interface_should_include_access_token_kind_comment()
    {
        var contents = File.ReadAllText(Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Abstractions",
            "Managers",
            "Generated",
            "IAuthorizationApi.g.cs"));

        contents.Should().Contain("Access token kind: partner");
        contents.Should().Contain("Required headers: x-tts-access-token, content-type");
    }
}
