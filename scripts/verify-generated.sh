#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT

dotnet run --project "$root/src/TikTokPartnerSdk.Generator" -- generate \
  "$root/tests/TikTokPartnerSdk.Tests/Fixtures/Schemas" \
  "$tmp"

diff -ru \
  --exclude 'Common' \
  --exclude 'Webhooks' \
  --exclude '*.csproj' \
  --exclude 'bin' \
  --exclude 'obj' \
  "$root/src/TikTokPartnerSdk.Generated" \
  "$tmp/src/TikTokPartnerSdk.Generated"
diff -ru "$root/src/TikTokPartnerSdk.Abstractions/Managers/Generated" "$tmp/src/TikTokPartnerSdk.Abstractions/Managers/Generated"
diff -ru "$root/src/TikTokPartnerSdk.Core/Managers/Generated" "$tmp/src/TikTokPartnerSdk.Core/Managers/Generated"
