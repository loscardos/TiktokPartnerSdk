namespace Loscardos.TikTokPartnerSdk.Storage.EntityFramework;

public interface ITikTokTokenProtector
{
    string Protect(string value);

    string Unprotect(string value);
}
