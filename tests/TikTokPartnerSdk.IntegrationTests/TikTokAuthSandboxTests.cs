using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Extensions.DependencyInjection;

namespace TikTokPartnerSdk.IntegrationTests;

public sealed class TikTokAuthSandboxTests
{
    [Fact]
    public void Sandbox_environment_loader_should_require_app_key_secret_and_redirect()
    {
        var configuration = TikTokSandboxEnvironment.TryLoad();

        Assert.NotNull(configuration);
    }

    [Fact]
    public void BuildAuthorizationUrl_should_use_sandbox_credentials()
    {
        var configuration = TikTokSandboxEnvironment.TryLoad();
        if (configuration is null)
        {
            return;
        }

        var services = new ServiceCollection();
        services.AddTikTokPartnerSdk(options =>
        {
            options.AppKey = configuration.AppKey;
            options.AppSecret = configuration.AppSecret;
        });

        using var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IAuthApi>();
        var url = api.BuildAuthorizationUrl(new Uri(configuration.RedirectUrl), "sandbox-state");

        Assert.Contains($"app_key={configuration.AppKey}", url.ToString(), StringComparison.Ordinal);
    }
}
