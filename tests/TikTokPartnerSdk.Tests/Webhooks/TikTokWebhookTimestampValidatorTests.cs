using FluentAssertions;
using Microsoft.Extensions.Options;
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;
using Loscardos.TikTokPartnerSdk.Core.Webhooks;

namespace Loscardos.TikTokPartnerSdk.Tests.Webhooks;

public sealed class TikTokWebhookTimestampValidatorTests
{
    [Fact]
    public void IsFresh_ReturnsTrueWhenTimestampIsInsideTolerance()
    {
        var validator = new TikTokWebhookTimestampValidator(Options.Create(new TikTokWebhookOptions
        {
            TimestampTolerance = TimeSpan.FromMinutes(5)
        }));
        var webhookEvent = new TikTokWebhookEvent(1, "n1", "s1", null, null, 1_700_000_000, default);

        validator.IsFresh(webhookEvent, DateTimeOffset.FromUnixTimeSeconds(1_700_000_120)).Should().BeTrue();
    }

    [Fact]
    public void IsFresh_ReturnsFalseWhenTimestampIsOlderThanTolerance()
    {
        var validator = new TikTokWebhookTimestampValidator(Options.Create(new TikTokWebhookOptions
        {
            TimestampTolerance = TimeSpan.FromMinutes(5)
        }));
        var webhookEvent = new TikTokWebhookEvent(1, "n1", "s1", null, null, 1_700_000_000, default);

        validator.IsFresh(webhookEvent, DateTimeOffset.FromUnixTimeSeconds(1_700_000_400)).Should().BeFalse();
    }

    [Fact]
    public void IsFresh_ReturnsFalseWhenTimestampIsMissing()
    {
        var validator = new TikTokWebhookTimestampValidator(Options.Create(new TikTokWebhookOptions()));
        var webhookEvent = new TikTokWebhookEvent(1, "n1", "s1", null, null, null, default);

        validator.IsFresh(webhookEvent, DateTimeOffset.FromUnixTimeSeconds(1_700_000_000)).Should().BeFalse();
    }
}
