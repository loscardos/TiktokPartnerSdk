using FluentAssertions;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Core.Auth;

namespace TikTokPartnerSdk.Tests.Auth;

public sealed class TikTokAuthApiTests
{
    [Fact]
    public void BuildAuthorizationUrl_should_include_app_key_and_state()
    {
        var api = new TikTokAuthApi(
            Options.Create(new TikTokPartnerOptions
            {
                AppKey = "app-key"
            }),
            client: new StubTikTokPartnerClient(),
            tokenStore: new InMemoryTikTokTokenStore());

        var url = api.BuildAuthorizationUrl(new Uri("https://example.com/callback"), "abc");

        url.ToString().Should().Contain("app_key=app-key");
        url.ToString().Should().Contain("state=abc");
    }

    private sealed class StubTikTokPartnerClient : ITikTokPartnerClient
    {
        public Task<TikTokPartnerResponseEnvelope<TResponse>> SendAsync<TResponse>(
            TikTokPartnerRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class InMemoryTikTokTokenStore : ITikTokTokenStore
    {
        public Task<TikTokTokenRecord?> GetAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
            => Task.FromResult<TikTokTokenRecord?>(null);

        public Task StoreAsync(TikTokTokenRecord token, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task ClearAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
