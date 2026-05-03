using TikTokPartnerSdk.Generated.Authorization;

namespace TikTokPartnerSdk.Abstractions.Managers.Generated;

public interface IAuthorizationApi
{
    Task<AuthorizationGetAuthorizedShopsResponse> GetAuthorizedShopsAsync(
        string accessToken,
        AuthorizationGetAuthorizedShopsRequest request,
        CancellationToken cancellationToken);
}
