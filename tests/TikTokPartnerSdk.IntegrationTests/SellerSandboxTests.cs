using Microsoft.Extensions.DependencyInjection;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Generated.Seller;

namespace TikTokPartnerSdk.IntegrationTests;

public sealed class SellerSandboxTests
{
    [TikTokSandboxFact]
    public async Task GetActiveShopsAsync_should_call_sandbox_when_credentials_are_available()
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
        var api = provider.GetRequiredService<ISellerApi>();

        var response = await api.GetActiveShopsAsync(
            token!.AccessToken,
            new SellerGetActiveShopsRequest(configuration.AppKey, 0, string.Empty),
            CancellationToken.None);

        Assert.Equal(0, response.Code);
    }
}
