using FluentAssertions;
using TikTokPartnerSdk.SampleConsole;

namespace TikTokPartnerSdk.Tests.SampleConsole;

public sealed class SandboxEnvFileTests
{
    [Fact]
    public void UpsertValues_should_update_existing_keys_and_append_missing_keys()
    {
        var path = Path.Combine(Path.GetTempPath(), $"tiktok-sandbox-{Guid.NewGuid():N}.env");
        File.WriteAllLines(path, new[]
        {
            "# local sandbox",
            "TIKTOK_SANDBOX_APP_KEY=app-key",
            "TIKTOK_SANDBOX_ACCESS_TOKEN=",
            "TIKTOK_SANDBOX_SHOP_CIPHER=old-cipher"
        });

        try
        {
            SandboxEnvFile.UpsertValues(path, new Dictionary<string, string>
            {
                ["TIKTOK_SANDBOX_ACCESS_TOKEN"] = "access-1",
                ["TIKTOK_SANDBOX_REFRESH_TOKEN"] = "refresh-1",
                ["TIKTOK_SANDBOX_SHOP_CIPHER"] = "new-cipher"
            });

            File.ReadAllText(path).Should().Be(string.Join(Environment.NewLine, new[]
            {
                "# local sandbox",
                "TIKTOK_SANDBOX_APP_KEY=app-key",
                "TIKTOK_SANDBOX_ACCESS_TOKEN=access-1",
                "TIKTOK_SANDBOX_SHOP_CIPHER=new-cipher",
                "TIKTOK_SANDBOX_REFRESH_TOKEN=refresh-1",
                string.Empty
            }));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
