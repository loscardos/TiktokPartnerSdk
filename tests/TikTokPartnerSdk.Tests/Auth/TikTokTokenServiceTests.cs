using FluentAssertions;
using Microsoft.Extensions.Options;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Configuration;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Core.Auth;

namespace TikTokPartnerSdk.Tests.Auth;

public sealed class TikTokTokenServiceTests
{
    [Fact]
    public async Task GetValidTokenAsync_should_return_existing_token_when_it_is_not_expiring()
    {
        var context = new TikTokAuthorizationContext(
            TikTokAccessTokenKind.Seller,
            AppKey: "app-key",
            ShopCipher: "shop-cipher");
        var existing = new TikTokTokenRecord(
            TikTokAccessTokenKind.Seller,
            AccessToken: "access-1",
            RefreshToken: "refresh-1",
            ExpiresAtUtc: DateTimeOffset.UtcNow.AddHours(1),
            RefreshTokenExpiresAtUtc: DateTimeOffset.UtcNow.AddDays(30),
            ShopCipher: "shop-cipher",
            AppKey: "app-key");
        var store = new InMemoryTikTokTokenStore(existing);
        var authApi = new StubAuthApi(existing);
        var service = new TikTokTokenService(
            store,
            authApi,
            Options.Create(new TikTokPartnerOptions()));

        var token = await service.GetValidTokenAsync(context, CancellationToken.None);

        token.AccessToken.Should().Be("access-1");
        authApi.RefreshCallCount.Should().Be(0);
    }

    private sealed class InMemoryTikTokTokenStore(TikTokTokenRecord? token) : ITikTokTokenStore
    {
        private TikTokTokenRecord? _token = token;

        public Task<TikTokTokenRecord?> GetAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
            => Task.FromResult(_token);

        public Task StoreAsync(TikTokTokenRecord token, CancellationToken cancellationToken)
        {
            _token = token;
            return Task.CompletedTask;
        }

        public Task ClearAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
        {
            _token = null;
            return Task.CompletedTask;
        }
    }

    private sealed class StubAuthApi(TikTokTokenRecord refreshToken) : IAuthApi
    {
        public int RefreshCallCount { get; private set; }

        public Uri BuildAuthorizationUrl(Uri redirectUri, string? state)
            => throw new NotSupportedException();

        public Task<TikTokTokenRecord> RefreshTokenAsync(
            TikTokAuthorizationContext context,
            CancellationToken cancellationToken)
        {
            RefreshCallCount++;
            return Task.FromResult(refreshToken);
        }
    }
}
