#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
output_dir="${1:-$root/artifacts/packages}"

mkdir -p "$output_dir"

dotnet pack "$root/src/TikTokPartnerSdk.Abstractions/TikTokPartnerSdk.Abstractions.csproj" -c Release -o "$output_dir"
dotnet pack "$root/src/TikTokPartnerSdk.Generated/TikTokPartnerSdk.Generated.csproj" -c Release -o "$output_dir"
dotnet pack "$root/src/TikTokPartnerSdk.Core/TikTokPartnerSdk.Core.csproj" -c Release -o "$output_dir"
dotnet pack "$root/src/TikTokPartnerSdk.Extensions.DependencyInjection/TikTokPartnerSdk.Extensions.DependencyInjection.csproj" -c Release -o "$output_dir"
dotnet pack "$root/src/TikTokPartnerSdk.Storage.EntityFramework/TikTokPartnerSdk.Storage.EntityFramework.csproj" -c Release -o "$output_dir"

echo "Packages written to $output_dir"
