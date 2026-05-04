namespace TikTokPartnerSdk.Abstractions.RateLimiting;

public interface ITikTokRateLimiter
{
    ValueTask WaitAsync(string partitionKey, CancellationToken cancellationToken);
}
