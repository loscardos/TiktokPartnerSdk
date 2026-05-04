# TikTokPartnerSdk

ASP.NET-friendly TikTok Shop Partner SDK for .NET 8.

Status: `0.1.0-preview`. The SDK is ready for local package smoke testing and starter sandbox integration work. It now has typed API errors, transient retry support, optional in-memory rate limiting, generated endpoint coverage guards, and a YAML documentation normalization path. Full production coverage is still in progress.

## Packages

- `TikTokPartnerSdk.Abstractions`
- `TikTokPartnerSdk.Generated`
- `TikTokPartnerSdk.Core`
- `TikTokPartnerSdk.Extensions.DependencyInjection`
- `TikTokPartnerSdk.Storage.EntityFramework`

## Documentation

- [Getting started](docs/getting-started.md)
- [Authentication](docs/auth.md)
- [Sandbox testing](docs/sandbox.md)
- [Endpoint coverage](docs/endpoints.md)
- [Token storage](docs/storage.md)
- [Runtime behavior](docs/runtime.md)
- [Generator](docs/generator.md)
- [Packaging](docs/packaging.md)

## Local Verification

```bash
dotnet test TikTokPartnerSdk.sln
bash scripts/verify-generated.sh
bash scripts/verify-packages.sh /tmp/tiktok-packages
```
