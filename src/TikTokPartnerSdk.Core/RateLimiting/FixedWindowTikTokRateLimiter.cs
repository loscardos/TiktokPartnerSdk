using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Abstractions.RateLimiting;

namespace Loscardos.TikTokPartnerSdk.Core.RateLimiting;

public sealed class FixedWindowTikTokRateLimiter(IOptions<TikTokPartnerOptions> options) : ITikTokRateLimiter
{
    private readonly object _gate = new();
    private readonly Dictionary<string, Queue<DateTimeOffset>> _windows = new(StringComparer.Ordinal);
    private readonly TikTokPartnerOptions _options = options.Value;

    public async ValueTask WaitAsync(string partitionKey, CancellationToken cancellationToken)
    {
        var key = string.IsNullOrWhiteSpace(partitionKey) ? "default" : partitionKey;

        while (true)
        {
            TimeSpan delay;
            lock (_gate)
            {
                var now = DateTimeOffset.UtcNow;
                var window = GetWindow(key);
                TrimExpired(window, now);

                if (window.Count < _options.RateLimitPermitLimit)
                {
                    window.Enqueue(now);
                    return;
                }

                var oldest = window.Peek();
                delay = oldest.Add(_options.RateLimitWindow) - now;
                if (delay < TimeSpan.Zero)
                {
                    delay = TimeSpan.Zero;
                }
            }

            await Task.Delay(delay, cancellationToken);
        }
    }

    private Queue<DateTimeOffset> GetWindow(string key)
    {
        if (!_windows.TryGetValue(key, out var window))
        {
            window = new Queue<DateTimeOffset>();
            _windows[key] = window;
        }

        return window;
    }

    private void TrimExpired(Queue<DateTimeOffset> window, DateTimeOffset now)
    {
        while (window.Count > 0 && now - window.Peek() >= _options.RateLimitWindow)
        {
            window.Dequeue();
        }
    }
}
