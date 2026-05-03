namespace TikTokPartnerSdk.Abstractions.Http;

public interface ITikTokPartnerClient
{
    Task<TikTokPartnerResponseEnvelope<TResponse>> SendAsync<TResponse>(
        TikTokPartnerRequest request,
        CancellationToken cancellationToken);
}
