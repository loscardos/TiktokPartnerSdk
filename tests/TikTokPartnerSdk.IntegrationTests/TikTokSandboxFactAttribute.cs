namespace Loscardos.TikTokPartnerSdk.IntegrationTests;

public sealed class TikTokSandboxFactAttribute : FactAttribute
{
    public TikTokSandboxFactAttribute()
    {
        var configuration = TikTokSandboxEnvironment.TryLoad();
        if (configuration is not { RunSandbox: true })
        {
            Skip = "Set TIKTOK_RUN_SANDBOX=true to run real TikTok sandbox tests.";
        }
    }
}
