using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Text.Json;
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

    [Fact]
    public async Task ExchangeCodeAsync_should_store_token_from_tiktok_response()
    {
        var store = new InMemoryTikTokTokenStore();
        var client = new StubTikTokPartnerClient(new TikTokPartnerResponseEnvelope<ExchangeCodeResponse>(
            0,
            "success",
            "req-1",
            new ExchangeCodeResponse("access-1", "refresh-1", 7200, 2592000)));
        var api = new TikTokAuthApi(
            Options.Create(new TikTokPartnerOptions
            {
                AppKey = "app-key",
                AppSecret = "app-secret"
            }),
            client,
            store);
        var context = new TikTokAuthorizationContext(TikTokAccessTokenKind.Seller, "app-key", "cipher-1");

        var token = await api.ExchangeCodeAsync(
            code: "code-1",
            context: context,
            cancellationToken: CancellationToken.None);

        token.AccessToken.Should().Be("access-1");
        (await store.GetAsync(context, CancellationToken.None)).Should().NotBeNull();
    }

    [Fact]
    public async Task RefreshTokenAsync_should_replace_expiring_token()
    {
        var context = new TikTokAuthorizationContext(TikTokAccessTokenKind.Seller, "app-key", "cipher-1");
        var existing = new TikTokTokenRecord(
            TikTokAccessTokenKind.Seller,
            "old-access",
            "old-refresh",
            DateTimeOffset.UtcNow.AddMinutes(-1),
            DateTimeOffset.UtcNow.AddDays(10),
            "cipher-1",
            "app-key");
        var store = new InMemoryTikTokTokenStore();
        await store.StoreAsync(existing, CancellationToken.None);
        var client = new StubTikTokPartnerClient(new TikTokPartnerResponseEnvelope<RefreshTokenResponse>(
            0,
            "success",
            "req-2",
            new RefreshTokenResponse("new-access", "new-refresh", 7200, 2592000)));
        var api = new TikTokAuthApi(
            Options.Create(new TikTokPartnerOptions
            {
                AppKey = "app-key",
                AppSecret = "app-secret"
            }),
            client,
            store);

        var refreshed = await api.RefreshTokenAsync(context, CancellationToken.None);

        refreshed.AccessToken.Should().Be("new-access");
    }

    private sealed class StubTikTokPartnerClient : ITikTokPartnerClient
    {
        private readonly object? _response;

        public StubTikTokPartnerClient(object? response = null)
        {
            _response = response;
        }

        public Task<TikTokPartnerResponseEnvelope<TResponse>> SendAsync<TResponse>(
            TikTokPartnerRequest request,
            CancellationToken cancellationToken)
        {
            if (_response is TikTokPartnerResponseEnvelope<TResponse> typed)
            {
                return Task.FromResult(typed);
            }

            if (_response is not null)
            {
                var json = JsonSerializer.Serialize(_response);
                var converted = JsonSerializer.Deserialize<TikTokPartnerResponseEnvelope<TResponse>>(json);
                if (converted is not null)
                {
                    return Task.FromResult(converted);
                }
            }

            throw new NotSupportedException();
        }
    }

    private sealed class InMemoryTikTokTokenStore : ITikTokTokenStore
    {
        private readonly Dictionary<string, TikTokTokenRecord> _tokens = new(StringComparer.Ordinal);

        public Task<TikTokTokenRecord?> GetAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
        {
            _tokens.TryGetValue(ToKey(context), out var token);
            return Task.FromResult<TikTokTokenRecord?>(token);
        }

        public Task StoreAsync(TikTokTokenRecord token, CancellationToken cancellationToken)
        {
            _tokens[ToKey(new TikTokAuthorizationContext(token.AccessTokenKind, token.AppKey, token.ShopCipher))] = token;
            return Task.CompletedTask;
        }

        public Task ClearAsync(TikTokAuthorizationContext context, CancellationToken cancellationToken)
        {
            _tokens.Remove(ToKey(context));
            return Task.CompletedTask;
        }

        private static string ToKey(TikTokAuthorizationContext context)
            => $"{context.AccessTokenKind}:{context.AppKey}:{context.ShopCipher ?? string.Empty}";
    }

    private sealed record ExchangeCodeResponse(
        string AccessToken,
        string RefreshToken,
        long AccessTokenExpireIn,
        long RefreshTokenExpireIn);

    private sealed record RefreshTokenResponse(
        string AccessToken,
        string RefreshToken,
        long AccessTokenExpireIn,
        long RefreshTokenExpireIn);
}
