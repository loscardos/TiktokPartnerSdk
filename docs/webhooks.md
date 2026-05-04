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
