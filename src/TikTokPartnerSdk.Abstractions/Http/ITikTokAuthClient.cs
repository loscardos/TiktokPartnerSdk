namespace TikTokPartnerSdk.Abstractions.Http;

public interface ITikTokAuthClient
{
    Task<TikTokPartnerResponseEnvelope<TResponse>> GetAsync<TResponse>(
        string path,
        object query,
        CancellationToken cancellationToken);
}
