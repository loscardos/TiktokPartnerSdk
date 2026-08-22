# Packaging

TikTokPartnerSdk packages are published under `Loscardos.TikTokPartnerSdk.*`.

| Package | Purpose |
| --- | --- |
| `Loscardos.TikTokPartnerSdk.Abstractions` | Public interfaces and shared contracts. |
| `Loscardos.TikTokPartnerSdk.Generated` | Generated request and response DTOs. |
| `Loscardos.TikTokPartnerSdk.Core` | HTTP, auth, signing, rate limiting, and manager implementations. |
| `Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection` | ASP.NET Core registration helpers. |
| `Loscardos.TikTokPartnerSdk.Storage.EntityFramework` | EF Core token storage. |

## Local Verification

```bash
dotnet test TikTokPartnerSdk.sln
./scripts/verify-generated.sh
./scripts/verify-packages.sh
```

`verify-packages.sh` packs the local projects, creates a clean consumer project, installs `Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection`, restores dependencies, and builds the consumer project.

## Version

The current preview version is:

```text
0.1.1-preview
```
