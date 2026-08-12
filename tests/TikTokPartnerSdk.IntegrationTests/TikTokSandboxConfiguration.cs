namespace Loscardos.TikTokPartnerSdk.IntegrationTests;

public sealed class TikTokSandboxConfiguration
{
    public bool RunSandbox { get; init; }
    public string AppKey { get; init; } = string.Empty;
    public string AppSecret { get; init; } = string.Empty;
    public string RedirectUrl { get; init; } = string.Empty;
    public string AuthCode { get; init; } = string.Empty;
    public string ShopCipher { get; init; } = string.Empty;
    public string PartnerAccessToken { get; init; } = string.Empty;
}
