using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Generator;

public sealed class ContractsWriterTests
{
    [Fact]
    public void Generated_authorization_contract_should_include_request_properties()
    {
        var path = Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Generated",
            "Authorization",
            "AuthorizationContracts.g.cs");

        File.Exists(path).Should().BeTrue();
        var contents = File.ReadAllText(path);

        contents.Should().Contain("AuthorizationGetAuthorizedShopsRequest");
        contents.Should().Contain("string AppKey");
        contents.Should().Contain("long Timestamp");
    }
}
