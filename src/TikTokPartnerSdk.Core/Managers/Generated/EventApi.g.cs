using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.Managers.Generated;

namespace TikTokPartnerSdk.Core.Managers.Generated;

public sealed class EventApi(ITikTokPartnerClient client) : IEventApi
{
    private readonly ITikTokPartnerClient _client = client;
}
