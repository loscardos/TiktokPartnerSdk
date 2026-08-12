using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;
using Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTikTokPartnerSdk(options =>
{
    options.AppKey = builder.Configuration["TikTok:AppKey"]
        ?? builder.Configuration["TIKTOK_SANDBOX_APP_KEY"]
        ?? "local-app-key";
    options.AppSecret = builder.Configuration["TikTok:AppSecret"]
        ?? builder.Configuration["TIKTOK_SANDBOX_APP_SECRET"]
        ?? "local-app-secret";
    options.WebhookSecret = builder.Configuration["TikTok:WebhookSecret"]
        ?? builder.Configuration["TIKTOK_SANDBOX_WEBHOOK_SECRET"]
        ?? string.Empty;
});
builder.Services.AddSingleton<InMemoryWebhookRawPayloadSink>();
builder.Services.AddSingleton<ITikTokWebhookRawPayloadSink>(provider
    => provider.GetRequiredService<InMemoryWebhookRawPayloadSink>());
builder.Services.AddSingleton<InMemoryWebhookQueue>();
builder.Services.AddSingleton<ITikTokWebhookQueue>(provider
    => provider.GetRequiredService<InMemoryWebhookQueue>());

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("TikTokWebhookReceiver");

app.MapPost("/webhooks/tiktok", async (
    HttpRequest request,
    [FromServices] ITikTokWebhookParser parser,
    [FromServices] ITikTokWebhookRawPayloadSink rawPayloadSink,
    [FromServices] ITikTokWebhookQueue queue,
    CancellationToken cancellationToken) =>
{
    using var reader = new StreamReader(request.Body);
    var rawBody = await reader.ReadToEndAsync(cancellationToken);
    var signature = request.Headers["x-tt-signature"].ToString();
    if (string.IsNullOrWhiteSpace(signature))
    {
        signature = request.Headers["TikTok-Signature"].ToString();
    }
    if (string.IsNullOrWhiteSpace(signature))
    {
        signature = request.Headers["authorization"].ToString();
    }

    var result = parser.TryReceive(request.Path.Value ?? string.Empty, rawBody, signature, DateTimeOffset.UtcNow);
    if (!result.IsAccepted)
    {
        logger.LogWarning(
            "Rejected TikTok webhook. reason={Reason} method={Method} path={Path} query={Query} host={Host} content_type={ContentType} signature_header={SignatureHeader} authorization_header={AuthorizationHeader} tiktok_headers={Headers} body={Body}",
            result.RejectionReason,
            request.Method,
            request.Path.Value,
            request.QueryString.Value,
            request.Host.Value,
            request.ContentType,
            request.Headers["TikTok-Signature"].ToString(),
            request.Headers["authorization"].ToString(),
            string.Join("; ", request.Headers
                .Where(header => header.Key.Contains("tiktok", StringComparison.OrdinalIgnoreCase)
                    || header.Key.Contains("tts", StringComparison.OrdinalIgnoreCase)
                    || header.Key.Contains("sign", StringComparison.OrdinalIgnoreCase)
                    || header.Key.Contains("auth", StringComparison.OrdinalIgnoreCase))
                .Select(header => header.Key + "=" + header.Value.ToString())),
            rawBody);
        return Results.Unauthorized();
    }

    await rawPayloadSink.SaveAsync(result.Envelope!, cancellationToken);
    await queue.EnqueueAsync(result.Envelope!, cancellationToken);

    return Results.Ok(new { accepted = true, result.Envelope!.IdempotencyKey });
});

app.MapGet("/webhooks/tiktok/queued", ([FromServices] InMemoryWebhookQueue queue)
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
