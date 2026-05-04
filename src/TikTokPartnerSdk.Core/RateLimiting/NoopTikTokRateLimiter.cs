using TikTokPartnerSdk.Abstractions.RateLimiting;

namespace TikTokPartnerSdk.Core.RateLimiting;

public sealed class NoopTikTokRateLimiter : ITikTokRateLimiter
{
    public ValueTask WaitAsync(string partitionKey, CancellationToken cancellationToken) => ValueTask.CompletedTask;
}
