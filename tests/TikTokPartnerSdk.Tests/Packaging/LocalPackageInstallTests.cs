using FluentAssertions;

namespace Loscardos.TikTokPartnerSdk.Tests.Packaging;

public sealed class LocalPackageInstallTests
{
    [Fact]
    public void Package_verification_scripts_should_exist_and_reference_pack_and_restore()
    {
        var packScript = File.ReadAllText(Path.Combine(TestPaths.RepositoryRoot, "scripts", "pack-local.sh"));
        var verifyScript = File.ReadAllText(Path.Combine(TestPaths.RepositoryRoot, "scripts", "verify-packages.sh"));

        packScript.Should().Contain("dotnet pack");
        verifyScript.Should().Contain("Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection");
        verifyScript.Should().Contain("dotnet add");
        verifyScript.Should().Contain("dotnet build");
        verifyScript.Should().Contain("pack-local.sh");
    }
}
