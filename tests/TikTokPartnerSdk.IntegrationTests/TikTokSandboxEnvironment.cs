using System.Collections;

namespace Loscardos.TikTokPartnerSdk.IntegrationTests;

public static class TikTokSandboxEnvironment
{
    public static TikTokSandboxConfiguration? TryLoad()
    {
        var values = LoadValues();
        var runSandbox = values.TryGetValue("TIKTOK_RUN_SANDBOX", out var enabled)
            && enabled.Equals("true", StringComparison.OrdinalIgnoreCase);

        string Get(string key) => values.TryGetValue(key, out var value) ? value : string.Empty;

        var configuration = new TikTokSandboxConfiguration
        {
            RunSandbox = runSandbox,
            AppKey = Get("TIKTOK_SANDBOX_APP_KEY"),
            AppSecret = Get("TIKTOK_SANDBOX_APP_SECRET"),
            RedirectUrl = Get("TIKTOK_SANDBOX_REDIRECT_URL"),
            AuthCode = Get("TIKTOK_SANDBOX_AUTH_CODE"),
            ShopCipher = Get("TIKTOK_SANDBOX_SHOP_CIPHER"),
            PartnerAccessToken = Get("TIKTOK_SANDBOX_PARTNER_ACCESS_TOKEN")
        };

        if (!runSandbox)
        {
            return configuration;
        }

        var missing = new[]
        {
            ("TIKTOK_SANDBOX_APP_KEY", configuration.AppKey),
            ("TIKTOK_SANDBOX_APP_SECRET", configuration.AppSecret),
            ("TIKTOK_SANDBOX_REDIRECT_URL", configuration.RedirectUrl)
        }
            .Where(static item => string.IsNullOrWhiteSpace(item.Item2))
            .Select(static item => item.Item1)
            .ToArray();

        if (missing.Length > 0)
        {
            throw new InvalidOperationException(
                "Missing required TikTok sandbox variables: " + string.Join(", ", missing));
        }

        return configuration;
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
