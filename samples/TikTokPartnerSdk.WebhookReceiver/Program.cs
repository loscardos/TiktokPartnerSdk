using System.Collections.Concurrent;
using TikTokPartnerSdk.Abstractions.Webhooks;
using TikTokPartnerSdk.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTikTokPartnerSdk(options =>
{
    options.AppKey = builder.Configuration["TikTok:AppKey"] ?? "local-app-key";
    options.AppSecret = builder.Configuration["TikTok:AppSecret"] ?? "local-app-secret";
});
builder.Services.AddSingleton<ITikTokWebhookRawPayloadSink, InMemoryWebhookRawPayloadSink>();
builder.Services.AddSingleton<ITikTokWebhookQueue, InMemoryWebhookQueue>();

var app = builder.Build();

app.MapPost("/webhooks/tiktok", async (
    HttpRequest request,
    ITikTokWebhookParser parser,
    ITikTokWebhookRawPayloadSink rawPayloadSink,
    ITikTokWebhookQueue queue,
    CancellationToken cancellationToken) =>
{
    using var reader = new StreamReader(request.Body);
    var rawBody = await reader.ReadToEndAsync(cancellationToken);
    var signature = request.Headers["TikTok-Signature"].ToString();
    if (string.IsNullOrWhiteSpace(signature))
    {
        signature = request.Headers["authorization"].ToString();
    }

    var result = parser.TryReceive(rawBody, signature, DateTimeOffset.UtcNow);
    if (!result.IsAccepted)
    {
        return Results.BadRequest(new { error = result.RejectionReason });
    }

    await rawPayloadSink.SaveAsync(result.Envelope!, cancellationToken);
    await queue.EnqueueAsync(result.Envelope!, cancellationToken);

    return Results.Ok(new { accepted = true, result.Envelope!.IdempotencyKey });
});

app.MapGet("/webhooks/tiktok/queued", (InMemoryWebhookQueue queue)
    => Results.Ok(queue.Envelopes.Select(x => new
    {
        x.IdempotencyKey,
        x.Event.Type,
        x.Event.NotificationId,
        x.Event.ShopId,
        x.ReceivedAt
    })));

app.Run();

internal sealed class InMemoryWebhookRawPayloadSink : ITikTokWebhookRawPayloadSink
{
    public ConcurrentQueue<TikTokWebhookEnvelope> Envelopes { get; } = new();

    public Task SaveAsync(TikTokWebhookEnvelope envelope, CancellationToken cancellationToken)
    {
        Envelopes.Enqueue(envelope);
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryWebhookQueue : ITikTokWebhookQueue
{
    public ConcurrentQueue<TikTokWebhookEnvelope> Envelopes { get; } = new();

    public Task EnqueueAsync(TikTokWebhookEnvelope envelope, CancellationToken cancellationToken)
    {
        Envelopes.Enqueue(envelope);
        return Task.CompletedTask;
    }
}
