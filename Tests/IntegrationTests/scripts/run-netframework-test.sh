#!/usr/bin/env bash
set -euo pipefail

package_version="${1:?QuestPDF package version is required}"
target_framework="${2:-net472}"
platform_target="${3:-AnyCPU}"

case "$target_framework" in
  net472|net481)
    ;;
  *)
    echo "Unknown .NET Framework target: $target_framework" >&2
    exit 2
    ;;
esac

case "$platform_target" in
  x64|x86|AnyCPU)
    ;;
  *)
    echo "Unknown platform target: $platform_target" >&2
    exit 2
    ;;
esac

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
integration_root="$repo_root/Tests/IntegrationTests"
project="$integration_root/QuestPDF.Tests.NetFramework/QuestPDF.Tests.NetFramework.csproj"
nuget_config="$integration_root/nuget.config"
artifacts_root="$repo_root/artifacts/integration/netframework/$target_framework/$platform_target"
output_dir="$artifacts_root/output"
pdf_file_name="questpdf-integration-$target_framework-$platform_target.pdf"

rm -rf "$artifacts_root"
mkdir -p "$output_dir"

dotnet restore "$project" \
  --configfile "$nuget_config" \
  -p:QuestPDFIntegrationVersion="$package_version" \
  -p:QuestPDFIntegrationTargetFramework="$target_framework" \
  -p:QuestPDFNetFrameworkTargetFramework="$target_framework" \
  -p:PlatformTarget="$platform_target"

dotnet build "$project" \
  --configuration Release \
  --framework "$target_framework" \
  --no-restore \
  -p:QuestPDFIntegrationVersion="$package_version" \
  -p:QuestPDFIntegrationTargetFramework="$target_framework" \
  -p:QuestPDFNetFrameworkTargetFramework="$target_framework" \
  -p:PlatformTarget="$platform_target"

exe="$integration_root/QuestPDF.Tests.NetFramework/bin/Release/$target_framework/QuestPDF.Tests.NetFramework.exe"
"$exe" "$output_dir" "$pdf_file_name" "$platform_target"

"$integration_root/scripts/validate-pdf.sh" "$output_dir/$pdf_file_name"
