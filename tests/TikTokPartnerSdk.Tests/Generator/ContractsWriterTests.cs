using FluentAssertions;

namespace TikTokPartnerSdk.Tests.Generator;

public sealed class ContractsWriterTests
{
    [Fact]
    public void Generated_contracts_should_include_authorization_request_and_response_types()
    {
        var path = Path.Combine(
            TestPaths.RepositoryRoot,
            "src",
            "TikTokPartnerSdk.Generated",
            "Authorization",
            "AuthorizationContracts.g.cs");

        File.Exists(path).Should().BeTrue();
        File.ReadAllText(path).Should().Contain("AuthorizationGetAuthorizedShopsRequest");
        File.ReadAllText(path).Should().Contain("AuthorizationGetAuthorizedShopsResponse");
    }
}
