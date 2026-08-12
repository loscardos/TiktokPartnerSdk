using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Loscardos.TikTokPartnerSdk.Storage.EntityFramework.Tests;

public sealed class DataProtectionTikTokTokenProtectorTests
{
    [Fact]
    public void Protect_RoundTripsWithoutStoringPlainText()
    {
        var services = new ServiceCollection();
        services.AddDataProtection();

        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<IDataProtectionProvider>();
        var protector = new DataProtectionTikTokTokenProtector(provider);

        var protectedValue = protector.Protect("secret-token");

        protectedValue.Should().NotBe("secret-token");
        protector.Unprotect(protectedValue).Should().Be("secret-token");
    }
}
