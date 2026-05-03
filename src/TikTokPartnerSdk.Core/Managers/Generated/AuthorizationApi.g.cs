using TikTokPartnerSdk.Abstractions.Http;
using TikTokPartnerSdk.Abstractions.Managers.Generated;
using TikTokPartnerSdk.Generated.Authorization;

namespace TikTokPartnerSdk.Core.Managers.Generated;

public sealed class AuthorizationApi(ITikTokPartnerClient client) : IAuthorizationApi
{
    public Task<AuthorizationGetAuthorizedShopsResponse> GetAuthorizedShopsAsync(
        string accessToken,
        AuthorizationGetAuthorizedShopsRequest request,
        CancellationToken cancellationToken)
    {
        _ = client;
        _ = accessToken;
        _ = request;
        _ = cancellationToken;
        throw new NotImplementedException();
    }
}
