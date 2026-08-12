# Event API

Use `IEventApi` to manage shop webhook subscriptions.

```csharp
using Loscardos.TikTokPartnerSdk.Generated.Event;
using EventApi = Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated.IEventApi;
```

## Generated Interface

```text
Loscardos.TikTokPartnerSdk.Abstractions.Managers.Generated.IEventApi
```

## DTO Namespace

```text
Loscardos.TikTokPartnerSdk.Generated.Event
```

## Common Operations

- Get shop webhooks.
- Update shop webhook.
- Delete shop webhook.

Webhook write operations should be tested with a controlled callback URL.

