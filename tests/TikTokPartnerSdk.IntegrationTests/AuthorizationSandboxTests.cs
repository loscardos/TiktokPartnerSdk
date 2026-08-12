using Microsoft.Extensions.DependencyInjection;
using Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated;
using Loscardos.TikTokPartnerSdk.Generated.Authorization;

namespace Loscardos.TikTokPartnerSdk.IntegrationTests;

public sealed class AuthorizationSandboxTests
{
    [TikTokSandboxFact]
    public async Task GetAuthorizedShopsAsync_should_call_sandbox_when_credentials_are_available()
    {
        var configuration = TikTokSandboxEnvironment.TryLoad()
            ?? throw new InvalidOperationException("TikTok sandbox configuration could not be loaded.");

        if (!TikTokSandboxTestSupport.HasSellerAuthorization(configuration))
        {
            throw new InvalidOperationException(
                "Seller sandbox tests require TIKTOK_SANDBOX_AUTH_CODE and TIKTOK_SANDBOX_SHOP_CIPHER.");
        }

        using var provider = TikTokSandboxTestSupport.CreateProvider(configuration);
        var token = await TikTokSandboxTestSupport.TryExchangeSellerTokenAsync(
            provider,
            configuration,
            CancellationToken.None);
        var api = provider.GetRequiredService<IAuthorizationApi>();

        var response = await api.GetAuthorizedShopsAsync(
            token!.AccessToken,
            new AuthorizationGetAuthorizedShopsRequest(configuration.AppKey, 0, string.Empty),
            CancellationToken.None);

        Assert.Equal(0, response.Code);
    }
}
