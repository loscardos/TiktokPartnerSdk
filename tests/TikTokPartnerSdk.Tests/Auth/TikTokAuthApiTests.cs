using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.Http;
using Loscardos.TikTokPartnerSdk.Core.Auth;

namespace Loscardos.TikTokPartnerSdk.Tests.Auth;

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
            authClient: new RecordingAuthClient(),
            tokenStore: new InMemoryTikTokTokenStore());

        var url = api.BuildAuthorizationUrl(new Uri("https://example.com/callback"), "abc");

        url.ToString().Should().Contain("app_key=app-key");
        url.ToString().Should().Contain("state=abc");
    }

    [Fact]
    public async Task ExchangeCodeAsync_should_store_token_from_tiktok_response()
    {
        var store = new InMemoryTikTokTokenStore();
        var authClient = new RecordingAuthClient(new TikTokPartnerResponseEnvelope<AuthTokenResponse>(
            0,
            "success",
            "req-1",
            new AuthTokenResponse("access-1", "refresh-1", 7200, 2592000)));
        var api = new TikTokAuthApi(
            Options.Create(new TikTokPartnerOptions
            {
                AppKey = "app-key",
                AppSecret = "app-secret"
            }),
            authClient,
            store);
        var context = new TikTokAuthorizationContext(TikTokAccessTokenKind.Seller, "app-key", "cipher-1");

        var token = await api.ExchangeCodeAsync(
            code: "code-1",
            context: context,
            cancellationToken: CancellationToken.None);

        token.AccessToken.Should().Be("access-1");
        authClient.LastPath.Should().Be("/token/get");
        authClient.LastQueryJson.Should().Contain("authorized_code");
        authClient.LastQueryJson.Should().Contain("auth_code");
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
        var authClient = new RecordingAuthClient(new TikTokPartnerResponseEnvelope<AuthTokenResponse>(
            0,
            "success",
            "req-2",
            new AuthTokenResponse("new-access", "new-refresh", 7200, 2592000)));
        var api = new TikTokAuthApi(
            Options.Create(new TikTokPartnerOptions
            {
                AppKey = "app-key",
                AppSecret = "app-secret"
            }),
            authClient,
            store);

        var refreshed = await api.RefreshTokenAsync(context, CancellationToken.None);

        refreshed.AccessToken.Should().Be("new-access");
        authClient.LastPath.Should().Be("/token/refresh");
        authClient.LastQueryJson.Should().Contain("old-refresh");
        authClient.LastQueryJson.Should().Contain("refresh_token");
    }

    [Fact]
    public async Task RefreshTokenAsync_with_explicit_token_should_leave_persistence_to_the_caller()
    {
        var context = new TikTokAuthorizationContext(
            TikTokAccessTokenKind.Seller,
            "app-key",
            "cipher-1");
        var existing = new TikTokTokenRecord(
            TikTokAccessTokenKind.Seller,
            "old-access",
            "old-refresh",
            DateTimeOffset.UtcNow.AddMinutes(-1),
            DateTimeOffset.UtcNow.AddDays(10),
            "cipher-1",
            "app-key");
        var store = new InMemoryTikTokTokenStore();
        var authClient = new RecordingAuthClient(
            new TikTokPartnerResponseEnvelope<AuthTokenResponse>(
                0,
                "success",
                "req-3",
                new AuthTokenResponse(
                    "new-access",
                    "new-refresh",
                    7200,
                    2592000)));
        var api = new TikTokAuthApi(
            Options.Create(new TikTokPartnerOptions
            {
                AppKey = "app-key",
                AppSecret = "app-secret"
            }),
            authClient,
            store);

        var refreshed = await api.RefreshTokenAsync(
            context,
            existing,
            CancellationToken.None);

        refreshed.AccessToken.Should().Be("new-access");
        authClient.LastQueryJson.Should().Contain("old-refresh");
        (await store.GetAsync(context, CancellationToken.None)).Should().BeNull();
    }

    private sealed class RecordingAuthClient : ITikTokAuthClient
    {
        private readonly object? _response;

        public RecordingAuthClient(object? response = null)
        {
            _response = response;
        }

        public string? LastPath { get; private set; }
        public string? LastQueryJson { get; private set; }

        public Task<TikTokPartnerResponseEnvelope<TResponse>> GetAsync<TResponse>(
            string path,
            object query,
            CancellationToken cancellationToken)
        {
            LastPath = path;
            LastQueryJson = JsonSerializer.Serialize(query);

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

    private sealed record AuthTokenResponse(
        [property: System.Text.Json.Serialization.JsonPropertyName("access_token")] string AccessToken,
        [property: System.Text.Json.Serialization.JsonPropertyName("refresh_token")] string RefreshToken,
        [property: System.Text.Json.Serialization.JsonPropertyName("access_token_expire_in")] long AccessTokenExpireIn,
        [property: System.Text.Json.Serialization.JsonPropertyName("refresh_token_expire_in")] long RefreshTokenExpireIn);
}
