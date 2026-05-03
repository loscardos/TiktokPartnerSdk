# TikTokPartnerSdk

ASP.NET-friendly TikTok Shop Partner SDK for .NET.

Packages:

- `TikTokPartnerSdk.Abstractions`
- `TikTokPartnerSdk.Core`
- `TikTokPartnerSdk.Generated`
- `TikTokPartnerSdk.Extensions.DependencyInjection`
- `TikTokPartnerSdk.Generator`

The TikTok SDK does not use YAML at runtime.
Generator inputs live under `tests/TikTokPartnerSdk.Tests/Fixtures/Schemas` as normalized JSON snapshots.
Generated DTOs and managers are committed under `src/` and shipped as compiled code.

Snapshot metadata now includes nested field trees plus auth requirements such as access token kind and required headers, and the generator projects that metadata into generated DTOs, coverage output, and manager source.
