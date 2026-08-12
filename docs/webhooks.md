# Webhooks

TikTokPartnerSdk provides TikTok Shop webhook primitives:

- verify raw callback payload signatures
- parse TikTok webhook envelopes
- validate callback timestamp freshness
- compute deterministic idempotency keys
- parse known webhook `type` values into typed payloads

The SDK intentionally does not own broker, storage, or worker infrastructure. A production receiver should persist or enqueue the verified raw envelope, return `200 OK` quickly, and process the event outside the HTTP request path.

Recommended receiver flow:

```text
TikTok Shop
  -> ASP.NET webhook endpoint
  -> read raw body
  -> verify signature
  -> validate timestamp tolerance
  -> compute idempotency key
  -> save or enqueue raw verified envelope
  -> return 200
  -> worker parses typed payload and runs business processing
```

The committed webhook specification snapshot is stored at `docs/webhooks/tiktok-webhooks.yaml`.

The verifier accepts TikTok Shop's webhook signature from `x-tt-signature` or `Authorization`. The signed payload is `app_key + raw-body`, hashed with HMAC-SHA256 using the app secret. Timestamped `TikTok-Signature` and raw-body HMAC signatures are still accepted for local compatibility tests.

## ASP.NET Receiver Example

The receiver should read the raw body before any JSON model binding changes it.

```csharp
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;

app.MapPost("/webhooks/tiktok", async (
    HttpRequest request,
    ITikTokWebhookParser parser,
    ITikTokWebhookRawPayloadSink rawPayloadSink,
    ITikTokWebhookQueue queue,
    CancellationToken cancellationToken) =>
{
    using var reader = new StreamReader(request.Body);
    var rawBody = await reader.ReadToEndAsync(cancellationToken);
    var signature = request.Headers["x-tt-signature"].ToString();
    if (string.IsNullOrWhiteSpace(signature))
    {
        signature = request.Headers["authorization"].ToString();
    }

    var result = parser.TryReceive(request.Path.Value ?? string.Empty, rawBody, signature, DateTimeOffset.UtcNow);
    if (!result.IsAccepted)
    {
        return Results.Unauthorized();
    }

    await rawPayloadSink.SaveAsync(result.Envelope!, cancellationToken);
    await queue.EnqueueAsync(result.Envelope!, cancellationToken);

    return Results.Ok();
});
```

`ITikTokWebhookRawPayloadSink` and `ITikTokWebhookQueue` are app-owned interfaces. The SDK defines them so receiver code can depend on stable contracts, but the application chooses its database, broker, retry policy, and worker topology.

## Worker Parsing Example

```csharp
using Loscardos.TikTokPartnerSdk.Abstractions.Webhooks;
using Loscardos.TikTokPartnerSdk.Generated.Webhooks;

public sealed class TikTokWebhookWorker(ITikTokWebhookParser parser)
{
    public void Process(TikTokWebhookEnvelope envelope)
    {
        if (envelope.Event.Type == TikTokWebhookType.OrderStatusChange)
        {
            var typed = parser.TryParseData<TikTokOrderStatusChangeWebhookData>(envelope);
            var orderId = typed?.Data.OrderId;
            var status = typed?.Data.OrderStatus;
        }
    }
}
```
