using Microsoft.AspNetCore.DataProtection;

namespace Loscardos.TikTokPartnerSdk.Storage.EntityFramework;

public sealed class DataProtectionTikTokTokenProtector : ITikTokTokenProtector
{
    private readonly IDataProtector _protector;

    public DataProtectionTikTokTokenProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("Loscardos.TikTokPartnerSdk.Storage.EntityFramework.Tokens.v1");
    }

    public string Protect(string value) => _protector.Protect(value);

    public string Unprotect(string value) => _protector.Unprotect(value);
}
