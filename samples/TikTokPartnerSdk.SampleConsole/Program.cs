using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Auth;
using TikTokPartnerSdk.Abstractions.Managers;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Extensions.DependencyInjection;
using TikTokPartnerSdk.Generated.Authorization;
using TikTokPartnerSdk.Generated.Event;
using TikTokPartnerSdk.Generated.Finance;
using TikTokPartnerSdk.Generated.Fulfillment;
using TikTokPartnerSdk.Generated.Logistics;
using TikTokPartnerSdk.Generated.ReturnAndRefund;
using TikTokPartnerSdk.Generated.Seller;
using TikTokPartnerSdk.SampleConsole;

var command = args.FirstOrDefault() ?? "help";
if (command is "help" or "--help" or "-h")
{
    PrintHelp();
    return;
}

try
{
    var env = LoadEnv();
    if (command == "check-config")
    {
        PrintConfigStatus(env);
        return;
    }

    using var provider = CreateProvider(env);

    switch (command)
    {
        case "auth-url":
            var authApi = provider.GetRequiredService<IAuthApi>();
            Console.WriteLine(authApi.BuildAuthorizationUrl(
                new Uri(Require(env, "TIKTOK_SANDBOX_REDIRECT_URL")),
                Get(env, "TIKTOK_SANDBOX_STATE", "sample-state")));
            break;

        case "exchange-code":
            var exchangeContext = CreateSellerAuthContext(env);
            var exchanged = await ExchangeSellerTokenAsync(provider, env, exchangeContext, CancellationToken.None);
            PrintToken(exchanged);
            break;

        case "refresh-token":
            var refreshContext = CreateSellerContext(env);
            await SeedTokenStoreAsync(provider, env, refreshContext, CancellationToken.None);
            var refreshed = await provider.GetRequiredService<IAuthApi>()
                .RefreshTokenAsync(refreshContext, CancellationToken.None);
            PersistToken(refreshed);
            PrintToken(refreshed);
            break;

        case "seller-shops":
        case "get-active-shops":
            await RunSellerShopsAsync(provider, env, CancellationToken.None);
            break;

        case "authorized-shops":
        case "get-authorized-shops":
            var authorizedContext = CreateSellerAuthContext(env);
            var authorizedToken = await SeedTokenStoreAsync(provider, env, authorizedContext, CancellationToken.None);
            await RunAuthorizedShopsWithTokenAsync(provider, authorizedToken.AccessToken, authorizedContext.AppKey, CancellationToken.None);
            break;

        case "orders-search":
            var ordersContext = CreateSellerContext(env);
            await SeedTokenStoreAsync(provider, env, ordersContext, CancellationToken.None);
            await RunOrdersSearchAsync(provider, ordersContext, env, CancellationToken.None);
            break;

        case "products-search":
            var productsContext = CreateSellerContext(env);
            await SeedTokenStoreAsync(provider, env, productsContext, CancellationToken.None);
            await RunProductsSearchAsync(provider, productsContext, env, CancellationToken.None);
            break;

        case "smoke":
            var smokeContext = CreateSellerContext(env);
            var smokeToken = await SeedTokenStoreAsync(provider, env, smokeContext, CancellationToken.None);
            await RunSmokeAsync(provider, smokeContext, smokeToken, env, CancellationToken.None);
            break;

        case "smoke-readonly":
        case "readonly-smoke":
            var readonlyContext = CreateSellerContext(env);
            var readonlyToken = await SeedTokenStoreAsync(provider, env, readonlyContext, CancellationToken.None);
            await RunReadonlySmokeAsync(provider, readonlyContext, readonlyToken, env, CancellationToken.None);
            break;

        default:
            Console.Error.WriteLine($"Unknown command '{command}'.");
            PrintHelp();
            Environment.ExitCode = 1;
            break;
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.Message);
    Environment.ExitCode = 1;
}

static ServiceProvider CreateProvider(IReadOnlyDictionary<string, string> env)
{
    var services = new ServiceCollection();
    services.AddTikTokPartnerSdk(options =>
    {
        options.AppKey = Require(env, "TIKTOK_SANDBOX_APP_KEY");
        options.AppSecret = Require(env, "TIKTOK_SANDBOX_APP_SECRET");
        options.RetryBaseDelay = TimeSpan.FromMilliseconds(100);
    });

    return services.BuildServiceProvider();
}

static TikTokAuthorizationContext CreateSellerContext(IReadOnlyDictionary<string, string> env)
    => new(
        TikTokAccessTokenKind.Seller,
        Require(env, "TIKTOK_SANDBOX_APP_KEY"),
        Require(env, "TIKTOK_SANDBOX_SHOP_CIPHER"));

static TikTokAuthorizationContext CreateSellerAuthContext(IReadOnlyDictionary<string, string> env)
    => new(
        TikTokAccessTokenKind.Seller,
        Require(env, "TIKTOK_SANDBOX_APP_KEY"),
        Get(env, "TIKTOK_SANDBOX_SHOP_CIPHER"));

static async Task<TikTokTokenRecord> SeedTokenStoreAsync(
    IServiceProvider provider,
    IReadOnlyDictionary<string, string> env,
    TikTokAuthorizationContext context,
    CancellationToken cancellationToken)
{
    var store = provider.GetRequiredService<ITikTokTokenStore>();
    var existing = await store.GetAsync(context, cancellationToken);
    if (existing is not null)
    {
        return existing;
    }

    var accessToken = Get(env, "TIKTOK_SANDBOX_ACCESS_TOKEN");
    if (!string.IsNullOrWhiteSpace(accessToken))
    {
        var token = new TikTokTokenRecord(
            TikTokAccessTokenKind.Seller,
            accessToken,
            Get(env, "TIKTOK_SANDBOX_REFRESH_TOKEN"),
            ParseDateTimeOffset(env, "TIKTOK_SANDBOX_ACCESS_TOKEN_EXPIRES_AT", DateTimeOffset.UtcNow.AddHours(1)),
            ParseDateTimeOffset(env, "TIKTOK_SANDBOX_REFRESH_TOKEN_EXPIRES_AT", DateTimeOffset.UtcNow.AddDays(30)),
            context.ShopCipher,
            context.AppKey);
        await store.StoreAsync(token, cancellationToken);
        return token;
    }

    if (!string.IsNullOrWhiteSpace(Get(env, "TIKTOK_SANDBOX_AUTH_CODE")))
    {
        return await ExchangeSellerTokenAsync(provider, env, context, cancellationToken);
    }

    throw new InvalidOperationException(
        "Token-aware commands require TIKTOK_SANDBOX_ACCESS_TOKEN or TIKTOK_SANDBOX_AUTH_CODE.");
}

static async Task<TikTokTokenRecord> ExchangeSellerTokenAsync(
    IServiceProvider provider,
    IReadOnlyDictionary<string, string> env,
    TikTokAuthorizationContext context,
    CancellationToken cancellationToken)
{
    var authApi = provider.GetRequiredService<IAuthApi>();
    var token = await authApi.ExchangeCodeAsync(
        Require(env, "TIKTOK_SANDBOX_AUTH_CODE"),
        context,
        cancellationToken);
    PersistToken(token);
    return token;
}

static async Task RunAuthorizedShopsWithTokenAsync(
    IServiceProvider provider,
    string accessToken,
    string appKey,
    CancellationToken cancellationToken)
{
    var api = provider.GetRequiredService<IAuthorizationApi>();
    var response = await api.GetAuthorizedShopsAsync(
        accessToken,
        new AuthorizationGetAuthorizedShopsRequest(
            appKey,
            0,
            string.Empty),
        cancellationToken);

    Console.WriteLine($"code={response.Code} request_id={response.RequestId} shops={response.Data.Shops.Count}");
    foreach (var shop in response.Data.Shops)
    {
        Console.WriteLine($"shop id={shop.Id} name={shop.Name} region={shop.Region} seller_type={shop.SellerType} cipher={shop.Cipher}");
    }
}

static async Task RunSellerShopsAsync(
    IServiceProvider provider,
    IReadOnlyDictionary<string, string> env,
    CancellationToken cancellationToken)
    => await RunSellerShopsWithTokenAsync(
        provider,
        Require(env, "TIKTOK_SANDBOX_ACCESS_TOKEN"),
        Require(env, "TIKTOK_SANDBOX_APP_KEY"),
        cancellationToken);

static async Task RunSellerShopsWithTokenAsync(
    IServiceProvider provider,
    string accessToken,
    string appKey,
    CancellationToken cancellationToken)
{
    var api = provider.GetRequiredService<ISellerApi>();
    var response = await api.GetActiveShopsAsync(
        accessToken,
        new SellerGetActiveShopsRequest(
            appKey,
            0,
            string.Empty),
        cancellationToken);

    Console.WriteLine($"code={response.Code} request_id={response.RequestId} shops={response.Data.Shops.Count}");
}

static async Task RunOrdersSearchAsync(
    IServiceProvider provider,
    TikTokAuthorizationContext context,
    IReadOnlyDictionary<string, string> env,
    CancellationToken cancellationToken)
{
    var manager = provider.GetRequiredService<IOrderManager>();
    var page = await manager.SearchOrdersAsync(
        context,
        new TikTokOrderSearchRequest(
            PageSize: ParseInt64(env, "TIKTOK_SANDBOX_PAGE_SIZE", 10),
            OrderStatus: Get(env, "TIKTOK_SANDBOX_ORDER_STATUS"),
            UpdateTimeGe: ParseInt64(env, "TIKTOK_SANDBOX_UPDATE_TIME_GE", 0),
            UpdateTimeLt: ParseInt64(env, "TIKTOK_SANDBOX_UPDATE_TIME_LT", 0)),
        cancellationToken);

    Console.WriteLine($"orders={page.Items.Count} total={page.TotalCount} next_page_token={page.NextPageToken}");
}

static async Task RunProductsSearchAsync(
    IServiceProvider provider,
    TikTokAuthorizationContext context,
    IReadOnlyDictionary<string, string> env,
    CancellationToken cancellationToken)
{
    var manager = provider.GetRequiredService<IProductManager>();
    var page = await manager.SearchProductsAsync(
        context,
        new TikTokProductSearchRequest(
            PageSize: ParseInt64(env, "TIKTOK_SANDBOX_PAGE_SIZE", 10),
            Status: Get(env, "TIKTOK_SANDBOX_PRODUCT_STATUS")),
        cancellationToken);

    Console.WriteLine($"products={page.Items.Count} total={page.TotalCount} next_page_token={page.NextPageToken}");
}

static async Task RunSmokeAsync(
    IServiceProvider provider,
    TikTokAuthorizationContext context,
    TikTokTokenRecord token,
    IReadOnlyDictionary<string, string> env,
    CancellationToken cancellationToken)
{
    var results = new List<bool>
    {
        await RunSmokeStepAsync("authorized-shops", () => RunAuthorizedShopsWithTokenAsync(provider, token.AccessToken, context.AppKey, cancellationToken)),
        await RunSmokeStepAsync("seller-shops", () => RunSellerShopsWithTokenAsync(provider, token.AccessToken, context.AppKey, cancellationToken)),
        await RunSmokeStepAsync("orders-search", () => RunOrdersSearchAsync(provider, context, env, cancellationToken)),
        await RunSmokeStepAsync("products-search", () => RunProductsSearchAsync(provider, context, env, cancellationToken))
    };

    if (results.Any(static passed => !passed))
    {
        Environment.ExitCode = 1;
    }
}

static async Task RunReadonlySmokeAsync(
    IServiceProvider provider,
    TikTokAuthorizationContext context,
    TikTokTokenRecord token,
    IReadOnlyDictionary<string, string> env,
    CancellationToken cancellationToken)
{
    await RunSmokeAsync(provider, context, token, env, cancellationToken);

    var appKey = context.AppKey;
    var shopCipher = RequireShopCipher(context);
    var pageSize = ParseInt64(env, "TIKTOK_SANDBOX_PAGE_SIZE", 10);
    var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    var from = now - (long)TimeSpan.FromDays(ParseInt64(env, "TIKTOK_SANDBOX_LOOKBACK_DAYS", 30)).TotalSeconds;

    var results = new List<bool>
    {
        await RunSmokeStepAsync("seller-permissions", async () =>
        {
            var api = provider.GetRequiredService<ISellerApi>();
            var response = await api.GetSellerPermissionsAsync(
                token.AccessToken,
                new SellerGetSellerPermissionsRequest(appKey, 0, string.Empty),
                cancellationToken);
            Console.WriteLine($"permissions={response.Data.Permissions?.Count ?? 0}");
        }),
        await RunSmokeStepAsync("event-webhooks", async () =>
        {
            var api = provider.GetRequiredService<IEventApi>();
            var response = await api.GetShopWebhooksAsync(
                token.AccessToken,
                new EventGetShopWebhooksRequest(appKey, 0, string.Empty, shopCipher),
                cancellationToken);
            Console.WriteLine($"webhooks={response.Data.Webhooks?.Count ?? 0} total={response.Data.TotalCount}");
        }),
        await RunSmokeStepAsync("logistics-warehouses", async () =>
        {
            var api = provider.GetRequiredService<ILogisticsApi>();
            var response = await api.GetWarehouseListAsync(
                token.AccessToken,
                new LogisticsGetWarehouseListRequest(appKey, 0, string.Empty, shopCipher),
                cancellationToken);
            Console.WriteLine($"warehouses={response.Data.Warehouses?.Count ?? 0}");
        }),
        await RunSmokeStepAsync("logistics-global-warehouses", async () =>
        {
            var api = provider.GetRequiredService<ILogisticsApi>();
            var response = await api.GetGlobalSellerWarehouseAsync(
                token.AccessToken,
                new LogisticsGetGlobalSellerWarehouseRequest(appKey, 0, string.Empty),
                cancellationToken);
            Console.WriteLine($"global_warehouses={response.Data.GlobalWarehouses?.Count ?? 0}");
        }),
        await RunSmokeStepAsync("fulfillment-packages-search", async () =>
        {
            var api = provider.GetRequiredService<IFulfillmentApi>();
            var response = await api.SearchPackageAsync(
                token.AccessToken,
                new FulfillmentSearchPackageRequest(
                    appKey,
                    0,
                    string.Empty,
                    pageSize,
                    string.Empty,
                    shopCipher,
                    "update_time",
                    "DESC",
                    from,
                    now,
                    from,
                    now,
                    null!),
                cancellationToken);
            Console.WriteLine($"packages={response.Data.Packages?.Count ?? 0} total={response.Data.TotalCount} next_page_token={response.Data.NextPageToken}");
        }),
        await RunSmokeStepAsync("returns-search-cancellations", async () =>
        {
            var api = provider.GetRequiredService<IReturnAndRefundApi>();
            var response = await api.SearchCancellationsAsync(
                token.AccessToken,
                new ReturnAndRefundSearchCancellationsRequest(
                    appKey,
                    0,
                    string.Empty,
                    pageSize.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    string.Empty,
                    shopCipher,
                    "update_time",
                    "DESC",
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    from,
                    now,
                    from,
                    now,
                    Get(env, "TIKTOK_SANDBOX_LOCALE", "en-US")),
                cancellationToken);
            Console.WriteLine($"cancellations={response.Data.Cancellations?.Count ?? 0} total={response.Data.TotalCount} next_page_token={response.Data.NextPageToken}");
        }),
        await RunSmokeStepAsync("returns-search-returns", async () =>
        {
            var api = provider.GetRequiredService<IReturnAndRefundApi>();
            var response = await api.SearchReturnsAsync(
                token.AccessToken,
                new ReturnAndRefundSearchReturnsRequest(
                    appKey,
                    0,
                    string.Empty,
                    pageSize.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    string.Empty,
                    shopCipher,
                    "update_time",
                    "DESC",
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    Array.Empty<string>(),
                    from,
                    Array.Empty<string>(),
                    from,
                    now,
                    Get(env, "TIKTOK_SANDBOX_LOCALE", "en-US"),
                    now),
                cancellationToken);
            Console.WriteLine($"returns={response.Data.ReturnOrders?.Count ?? 0} total={response.Data.TotalCount} next_page_token={response.Data.NextPageToken}");
        }),
        await RunSmokeStepAsync("finance-payments", async () =>
        {
            var api = provider.GetRequiredService<IFinanceApi>();
            var response = await api.GetPaymentsAsync(
                token.AccessToken,
                new FinanceGetPaymentsRequest(appKey, 0, string.Empty, from, now, pageSize, string.Empty, shopCipher, "create_time", "DESC"),
                cancellationToken);
            Console.WriteLine($"payments={response.Data.Payments?.Count ?? 0} next_page_token={response.Data.NextPageToken}");
        }),
        await RunSmokeStepAsync("finance-statements", async () =>
        {
            var api = provider.GetRequiredService<IFinanceApi>();
            var response = await api.GetStatementsAsync(
                token.AccessToken,
                new FinanceGetStatementsRequest(appKey, 0, string.Empty, pageSize, string.Empty, null!, shopCipher, "statement_time", "DESC", from, now),
                cancellationToken);
            Console.WriteLine($"statements={response.Data.Statements?.Count ?? 0} next_page_token={response.Data.NextPageToken}");
        }),
        await RunSmokeStepAsync("finance-withdrawals", async () =>
        {
            var api = provider.GetRequiredService<IFinanceApi>();
            var response = await api.GetWithdrawalsAsync(
                token.AccessToken,
                new FinanceGetWithdrawalsRequest(appKey, 0, string.Empty, from, now, pageSize, string.Empty, shopCipher, ["WITHDRAW", "SETTLE"]),
                cancellationToken);
            Console.WriteLine($"withdrawals={response.Data.Withdrawals?.Count ?? 0} total={response.Data.TotalCount} next_page_token={response.Data.NextPageToken}");
        }),
        await RunSmokeStepAsync("finance-unsettled-transactions", async () =>
        {
            var api = provider.GetRequiredService<IFinanceApi>();
            var response = await api.GetUnsettledTransactionsAsync(
                token.AccessToken,
                new FinanceGetUnsettledTransactionsRequest(appKey, 0, string.Empty, pageSize, string.Empty, from, now, shopCipher, "order_create_time", "DESC"),
                cancellationToken);
            Console.WriteLine($"unsettled_transactions={response.Data.Transactions?.Count ?? 0} total={response.Data.TotalCount} next_page_token={response.Data.NextPageToken}");
        })
    };

    if (results.Any(static passed => !passed))
    {
        Environment.ExitCode = 1;
    }
}

static async Task<bool> RunSmokeStepAsync(string name, Func<Task> action)
{
    Console.WriteLine($"== {name} ==");
    try
    {
        await action();
        Console.WriteLine($"PASS {name}");
        return true;
    }
    catch (Exception exception)
    {
        Console.WriteLine($"FAIL {name}: {exception.Message}");
        return false;
    }
}

static void PrintConfigStatus(IReadOnlyDictionary<string, string> env)
{
    var keys = new[]
    {
        "TIKTOK_SANDBOX_APP_KEY",
        "TIKTOK_SANDBOX_APP_SECRET",
        "TIKTOK_SANDBOX_REDIRECT_URL",
        "TIKTOK_SANDBOX_SHOP_CIPHER",
        "TIKTOK_SANDBOX_AUTH_CODE",
        "TIKTOK_SANDBOX_ACCESS_TOKEN",
        "TIKTOK_SANDBOX_REFRESH_TOKEN"
    };

    foreach (var key in keys)
    {
        Console.WriteLine($"{key}={(string.IsNullOrWhiteSpace(Get(env, key)) ? "missing" : "set")}");
    }
}

static void PrintToken(TikTokTokenRecord token)
{
    Console.WriteLine($"access_token={Mask(token.AccessToken)}");
    Console.WriteLine($"refresh_token={Mask(token.RefreshToken)}");
    Console.WriteLine($"access_token_expires_at={token.ExpiresAtUtc:O}");
    Console.WriteLine($"refresh_token_expires_at={token.RefreshTokenExpiresAtUtc:O}");
    Console.WriteLine($"shop_cipher={token.ShopCipher}");
}

static void PersistToken(TikTokTokenRecord token)
{
    var values = new Dictionary<string, string>
    {
        ["TIKTOK_SANDBOX_ACCESS_TOKEN"] = token.AccessToken,
        ["TIKTOK_SANDBOX_REFRESH_TOKEN"] = token.RefreshToken,
        ["TIKTOK_SANDBOX_ACCESS_TOKEN_EXPIRES_AT"] = token.ExpiresAtUtc.ToString("O"),
        ["TIKTOK_SANDBOX_REFRESH_TOKEN_EXPIRES_AT"] = token.RefreshTokenExpiresAtUtc.ToString("O"),
        ["TIKTOK_SANDBOX_AUTH_CODE"] = string.Empty
    };

    if (!string.IsNullOrWhiteSpace(token.ShopCipher))
    {
        values["TIKTOK_SANDBOX_SHOP_CIPHER"] = token.ShopCipher;
    }

    SandboxEnvFile.UpsertValues(SandboxEnvFile.DefaultPath, values);
}

static void PrintHelp()
{
    Console.WriteLine("TikTokPartnerSdk sample validation CLI");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  check-config        Show which sandbox variables are available");
    Console.WriteLine("  auth-url            Build the Partner Center authorization URL");
    Console.WriteLine("  exchange-code       Exchange TIKTOK_SANDBOX_AUTH_CODE for a seller token");
    Console.WriteLine("  refresh-token       Refresh a seeded or exchanged seller token");
    Console.WriteLine("  authorized-shops    Call Authorization get authorized shops with an access token");
    Console.WriteLine("  seller-shops        Call Seller get active shops with an access token");
    Console.WriteLine("  orders-search       Search orders through token-aware IOrderManager");
    Console.WriteLine("  products-search     Search products through token-aware IProductManager");
    Console.WriteLine("  smoke               Run direct read-only validation: shops, orders, products");
    Console.WriteLine("  smoke-readonly      Run broader read-only validation across seller, event, logistics, fulfillment, returns, finance");
}

static string Require(IReadOnlyDictionary<string, string> values, string key)
{
    var value = Get(values, key);
    return !string.IsNullOrWhiteSpace(value)
        ? value
        : throw new InvalidOperationException($"Missing required environment value '{key}'.");
}

static string RequireShopCipher(TikTokAuthorizationContext context)
    => !string.IsNullOrWhiteSpace(context.ShopCipher)
        ? context.ShopCipher
        : throw new InvalidOperationException("Shop cipher is required for seller shop APIs.");

static string Get(IReadOnlyDictionary<string, string> values, string key, string fallback = "")
    => values.TryGetValue(key, out var value) ? value : fallback;

static long ParseInt64(IReadOnlyDictionary<string, string> values, string key, long fallback)
    => long.TryParse(Get(values, key), out var value) ? value : fallback;

static DateTimeOffset ParseDateTimeOffset(
    IReadOnlyDictionary<string, string> values,
    string key,
    DateTimeOffset fallback)
    => DateTimeOffset.TryParse(Get(values, key), out var value) ? value : fallback;

static string Mask(string value)
    => string.IsNullOrWhiteSpace(value)
        ? string.Empty
        : value.Length <= 8 ? "********" : $"{value[..4]}...{value[^4..]}";

static Dictionary<string, string> LoadEnv()
{
    var values = Environment.GetEnvironmentVariables()
        .Cast<System.Collections.DictionaryEntry>()
        .Where(static entry => entry.Key is string && entry.Value is string)
        .ToDictionary(
            static entry => (string)entry.Key,
            static entry => (string)entry.Value!,
            StringComparer.Ordinal);

    var path = SandboxEnvFile.DefaultPath;
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
            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim();
            if (!string.IsNullOrWhiteSpace(value) || !values.ContainsKey(key))
            {
                values[key] = value;
            }
        }
    }

    return values;
}
