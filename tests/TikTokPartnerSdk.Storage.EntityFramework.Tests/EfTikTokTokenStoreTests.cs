using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TikTokPartnerSdk.Abstractions.Auth;

namespace TikTokPartnerSdk.Storage.EntityFramework.Tests;

public sealed class EfTikTokTokenStoreTests
{
    [Fact]
    public async Task StoreAsync_UpsertsAndReadsTokenByAuthorizationContext()
    {
        var options = new DbContextOptionsBuilder<TikTokTokenDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new TikTokTokenDbContext(options);
        var store = new EfTikTokTokenStore(db, new PlainTextTikTokTokenProtector());
        var token = new TikTokTokenRecord(
            TikTokAccessTokenKind.Seller,
            "access",
            "refresh",
            DateTimeOffset.UtcNow.AddHours(4),
            DateTimeOffset.UtcNow.AddDays(30),
            "cipher-1",
            "app-key");

        await store.StoreAsync(token, CancellationToken.None);
        var loaded = await store.GetAsync(
            new TikTokAuthorizationContext(TikTokAccessTokenKind.Seller, "app-key", "cipher-1"),
            CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.AccessToken.Should().Be("access");
        loaded.RefreshToken.Should().Be("refresh");
        loaded.AppKey.Should().Be("app-key");
        loaded.ShopCipher.Should().Be("cipher-1");
    }

    [Fact]
    public async Task ClearAsync_RemovesStoredToken()
    {
        var options = new DbContextOptionsBuilder<TikTokTokenDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new TikTokTokenDbContext(options);
        var store = new EfTikTokTokenStore(db, new PlainTextTikTokTokenProtector());
        var context = new TikTokAuthorizationContext(TikTokAccessTokenKind.Partner, "app-key");

        await store.StoreAsync(
            new TikTokTokenRecord(
                TikTokAccessTokenKind.Partner,
                "access",
                "refresh",
                DateTimeOffset.UtcNow.AddHours(1),
                DateTimeOffset.UtcNow.AddDays(7),
                null,
                "app-key"),
            CancellationToken.None);

        await store.ClearAsync(context, CancellationToken.None);

        var loaded = await store.GetAsync(context, CancellationToken.None);
        loaded.Should().BeNull();
    }
}
