using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Extensions.DependencyInjection;
using TikTokPartnerSdk.Generated.Authorization;
using TikTokPartnerSdk.Generated.Seller;

var command = args.FirstOrDefault() ?? "help";
if (command is "help" or "--help" or "-h")
{
    PrintHelp();
    return;
}

var env = LoadEnv();
var services = new ServiceCollection();
services.AddTikTokPartnerSdk(options =>
{
    options.AppKey = Require(env, "TIKTOK_SANDBOX_APP_KEY");
    options.AppSecret = Require(env, "TIKTOK_SANDBOX_APP_SECRET");
});

using var provider = services.BuildServiceProvider();

switch (command)
{
    case "auth-url":
        var authApi = provider.GetRequiredService<IAuthApi>();
        Console.WriteLine(authApi.BuildAuthorizationUrl(
            new Uri(Require(env, "TIKTOK_SANDBOX_REDIRECT_URL")),
            "sample-state"));
        break;

    case "exchange-code":
        var exchangeApi = provider.GetRequiredService<IAuthApi>();
        var token = await exchangeApi.ExchangeCodeAsync(
            Require(env, "TIKTOK_SANDBOX_AUTH_CODE"),
            new TikTokAuthorizationContext(
                TikTokAccessTokenKind.Seller,
                Require(env, "TIKTOK_SANDBOX_APP_KEY"),
                Require(env, "TIKTOK_SANDBOX_SHOP_CIPHER")),
            CancellationToken.None);
        Console.WriteLine($"access_token_expires_at={token.ExpiresAtUtc:O}");
        break;

    case "get-authorized-shops":
        var authorizationApi = provider.GetRequiredService<IAuthorizationApi>();
        var shops = await authorizationApi.GetAuthorizedShopsAsync(
            Require(env, "TIKTOK_SANDBOX_ACCESS_TOKEN"),
            new AuthorizationGetAuthorizedShopsRequest(
                Require(env, "TIKTOK_SANDBOX_APP_KEY"),
                0,
                string.Empty),
            CancellationToken.None);
        Console.WriteLine($"code={shops.Code} request_id={shops.RequestId}");
        break;

    case "get-active-shops":
        var sellerApi = provider.GetRequiredService<ISellerApi>();
        var activeShops = await sellerApi.GetActiveShopsAsync(
            Require(env, "TIKTOK_SANDBOX_ACCESS_TOKEN"),
            new SellerGetActiveShopsRequest(
                Require(env, "TIKTOK_SANDBOX_APP_KEY"),
                0,
                string.Empty),
            CancellationToken.None);
        Console.WriteLine($"code={activeShops.Code} request_id={activeShops.RequestId}");
        break;

    default:
        Console.Error.WriteLine($"Unknown command '{command}'.");
        PrintHelp();
        Environment.ExitCode = 1;
        break;
}

static void PrintHelp()
{
    Console.WriteLine("Commands: auth-url, exchange-code, get-authorized-shops, get-active-shops");
}

static string Require(IReadOnlyDictionary<string, string> values, string key)
{
    if (values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
    {
        return value;
    }

    throw new InvalidOperationException($"Missing required environment value '{key}'.");
}

static Dictionary<string, string> LoadEnv()
{
    var values = Environment.GetEnvironmentVariables()
        .Cast<System.Collections.DictionaryEntry>()
        .Where(static entry => entry.Key is string && entry.Value is string)
        .ToDictionary(
            static entry => (string)entry.Key,
            static entry => (string)entry.Value!,
            StringComparer.Ordinal);

    var path = Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory,
        "..",
        "..",
        "..",
        "..",
        "..",
        ".token",
        "tiktok-sandbox.env"));
    if (!File.Exists(path))
    {
        return values;
    }

    foreach (var line in File.ReadAllLines(path))
    {
        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#", StringComparison.Ordinal))
        {
            continue;
        }

        var separator = line.IndexOf('=', StringComparison.Ordinal);
        if (separator > 0)
        {
            values[line[..separator].Trim()] = line[(separator + 1)..].Trim();
        }
    }

    return values;
}
