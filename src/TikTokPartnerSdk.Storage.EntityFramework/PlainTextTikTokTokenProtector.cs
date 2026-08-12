namespace Loscardos.TikTokPartnerSdk.Storage.EntityFramework;

public sealed class PlainTextTikTokTokenProtector : ITikTokTokenProtector
{
    public string Protect(string value) => value;

    public string Unprotect(string value) => value;
}
