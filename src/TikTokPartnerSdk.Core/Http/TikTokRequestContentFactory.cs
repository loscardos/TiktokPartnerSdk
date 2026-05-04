using System.Text;

namespace TikTokPartnerSdk.Core.Http;

public sealed class TikTokRequestContentFactory
{
    public HttpContent? Create(object? body)
    {
        if (body is null)
        {
            return null;
        }

        var json = TikTokRequestBodySerializer.Serialize(body);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
