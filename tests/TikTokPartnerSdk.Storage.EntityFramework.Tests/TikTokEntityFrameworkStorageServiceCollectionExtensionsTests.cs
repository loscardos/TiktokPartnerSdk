using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Loscardos.TikTokPartnerSdk.Abstractions.Auth;

namespace Loscardos.TikTokPartnerSdk.Storage.EntityFramework.Tests;

public sealed class TikTokEntityFrameworkStorageServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTikTokEntityFrameworkTokenStorage_RegistersEncryptedTokenStore()
    {
        var services = new ServiceCollection();
        services.AddDataProtection();
        services.AddDbContext<TikTokTokenDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddTikTokEntityFrameworkTokenStorage();

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ITikTokTokenStore>().Should().BeOfType<EfTikTokTokenStore>();
        provider.GetRequiredService<ITikTokTokenProtector>().Should().BeOfType<DataProtectionTikTokTokenProtector>();
    }
}
