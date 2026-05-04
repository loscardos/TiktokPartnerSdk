#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
output_dir="${1:-$root/artifacts/packages}"
consumer_dir="${2:-$root/.tmp/package-consumer}"

bash "$root/scripts/pack-local.sh" "$output_dir"

rm -rf "$consumer_dir"
mkdir -p "$consumer_dir"

dotnet new console -n TikTokPartnerSdk.PackageSmoke -o "$consumer_dir/TikTokPartnerSdk.PackageSmoke" --force >/dev/null

cat > "$consumer_dir/NuGet.config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$output_dir" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
EOF

dotnet add "$consumer_dir/TikTokPartnerSdk.PackageSmoke/TikTokPartnerSdk.PackageSmoke.csproj" package TikTokPartnerSdk.Extensions.DependencyInjection --version 0.1.0 --source "$output_dir"
dotnet build "$consumer_dir/TikTokPartnerSdk.PackageSmoke/TikTokPartnerSdk.PackageSmoke.csproj" --configfile "$consumer_dir/NuGet.config"

echo "Package verification succeeded using $output_dir"
