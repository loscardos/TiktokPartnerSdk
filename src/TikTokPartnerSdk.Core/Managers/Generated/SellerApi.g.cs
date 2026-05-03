using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.Managers.Generated;

namespace TikTokPartnerSdk.Core.Managers.Generated;

public sealed class SellerApi(ITikTokPartnerClient client) : ISellerApi
{
    private readonly ITikTokPartnerClient _client = client;
}
