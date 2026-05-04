# Sandbox Validation

Use the sample console for local sandbox validation.

```bash
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- check-config
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- auth-url
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- exchange-code
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- refresh-token
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- smoke-readonly
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- certify-readonly
dotnet run --project samples/TikTokPartnerSdk.SampleConsole -- certify-matrix
```

## Environment File

Store sandbox values in ignored local files such as `.token/tiktok-sandbox.env`.

```bash
TIKTOK_SANDBOX_APP_KEY=
TIKTOK_SANDBOX_APP_SECRET=
TIKTOK_SANDBOX_REDIRECT_URL=
TIKTOK_SANDBOX_AUTH_CODE=
TIKTOK_SANDBOX_SHOP_CIPHER=
TIKTOK_SANDBOX_ACCESS_TOKEN=
TIKTOK_SANDBOX_REFRESH_TOKEN=
TIKTOK_SANDBOX_ACCESS_TOKEN_EXPIRES_AT=
TIKTOK_SANDBOX_REFRESH_TOKEN_EXPIRES_AT=
```

## Certification Commands

| Command | Purpose |
| --- | --- |
| `smoke-readonly` | Calls a broad read-only subset with live sandbox credentials. |
| `certify-readonly` | Writes a read-only certification report under `.tmp`. |
| `certify-matrix` | Writes an endpoint certification matrix for all generated endpoints. |
| `certify-write --i-understand-this-mutates-state` | Writes the mutation endpoint fixture matrix. It does not mutate by default. |

Reports under `.tmp` are runtime artifacts and should not be committed.

