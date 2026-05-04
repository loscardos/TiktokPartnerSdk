using System.Collections;

namespace TikTokPartnerSdk.IntegrationTests;

public static class TikTokSandboxEnvironment
{
    public static TikTokSandboxConfiguration? TryLoad()
    {
        var values = LoadValues();

        if (!values.TryGetValue("TIKTOK_SANDBOX_APP_KEY", out var appKey)
            || string.IsNullOrWhiteSpace(appKey)
            || !values.TryGetValue("TIKTOK_SANDBOX_APP_SECRET", out var appSecret)
            || string.IsNullOrWhiteSpace(appSecret)
            || !values.TryGetValue("TIKTOK_SANDBOX_REDIRECT_URL", out var redirectUrl)
            || string.IsNullOrWhiteSpace(redirectUrl))
        {
            return null;
        }

        values.TryGetValue("TIKTOK_SANDBOX_AUTH_CODE", out var authCode);
        values.TryGetValue("TIKTOK_SANDBOX_SHOP_CIPHER", out var shopCipher);

        return new TikTokSandboxConfiguration
        {
            AppKey = appKey,
            AppSecret = appSecret,
            RedirectUrl = redirectUrl,
            AuthCode = authCode ?? string.Empty,
            ShopCipher = shopCipher ?? string.Empty
        };
    }

    private static Dictionary<string, string> LoadValues()
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            if (entry.Key is string key && entry.Value is string value)
            {
                values[key] = value;
            }
        }

        var envFile = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".token", "tiktok-sandbox.env");
        envFile = Path.GetFullPath(envFile);
        if (!File.Exists(envFile))
        {
            return values;
        }

        foreach (var line in File.ReadAllLines(envFile))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var separator = line.IndexOf('=', StringComparison.Ordinal);
            if (separator <= 0)
            {
                continue;
            }

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();
            if (!values.ContainsKey(key))
            {
                values[key] = value;
            }
        }

        return values;
    }
}
