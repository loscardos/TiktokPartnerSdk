#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
output_dir="${1:-$root/artifacts/packages}"
consumer_dir="${2:-$root/.tmp/package-consumer}"
version="${3:-$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$root/Directory.Build.props" | head -n 1)}"

if [[ -z "$version" ]]; then
  echo "Unable to determine package version from Directory.Build.props" >&2
  exit 1
fi

bash "$root/scripts/pack-local.sh" "$output_dir"

rm -rf "$consumer_dir"
mkdir -p "$consumer_dir"
export NUGET_PACKAGES="$consumer_dir/.nuget/packages"

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

dotnet add "$consumer_dir/TikTokPartnerSdk.PackageSmoke/TikTokPartnerSdk.PackageSmoke.csproj" package Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection --version "$version" --no-restore

cat > "$consumer_dir/TikTokPartnerSdk.PackageSmoke/Program.cs" <<'CS'
using Microsoft.Extensions.DependencyInjection;
using Loscardos.TikTokPartnerSdk.Abstractions.Configuration;
using Loscardos.TikTokPartnerSdk.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddTikTokPartnerSdk(options =>
{
    options.AppKey = "package-smoke";
    options.AppSecret = "package-smoke";
});

Console.WriteLine(typeof(TikTokPartnerOptions).FullName);
CS

dotnet restore "$consumer_dir/TikTokPartnerSdk.PackageSmoke/TikTokPartnerSdk.PackageSmoke.csproj" --configfile "$consumer_dir/NuGet.config"
dotnet build "$consumer_dir/TikTokPartnerSdk.PackageSmoke/TikTokPartnerSdk.PackageSmoke.csproj" --no-restore

echo "Package verification succeeded using $output_dir (version $version)"
