namespace TikTokPartnerSdk.Abstractions.Http;

public interface ITikTokAuthClient
{
    Task<TikTokPartnerResponseEnvelope<TResponse>> PostAsync<TResponse>(
        string path,
        object body,
        CancellationToken cancellationToken);
}
