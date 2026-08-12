using Loscardos.TikTokPartnerSdk.Abstractions.RateLimiting;

namespace Loscardos.TikTokPartnerSdk.Core.RateLimiting;

public sealed class NoopTikTokRateLimiter : ITikTokRateLimiter
{
    public ValueTask WaitAsync(string partitionKey, CancellationToken cancellationToken) => ValueTask.CompletedTask;
}
