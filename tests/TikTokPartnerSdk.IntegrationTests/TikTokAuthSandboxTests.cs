using Microsoft.Extensions.DependencyInjection;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers;
using Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection;

namespace Loscardos.TikTokPartnerSdk.IntegrationTests;

public sealed class TikTokAuthSandboxTests
{
    [TikTokSandboxFact]
    public void BuildAuthorizationUrl_should_use_sandbox_credentials()
    {
        var configuration = TikTokSandboxEnvironment.TryLoad()
            ?? throw new InvalidOperationException("TikTok sandbox configuration could not be loaded.");

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
