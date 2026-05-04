# TikTokPartnerSdk

ASP.NET-friendly TikTok Shop Partner SDK for .NET 8.

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
