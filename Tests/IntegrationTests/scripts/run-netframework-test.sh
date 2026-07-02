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
output_dir="$repo_root/artifacts/integration-output/netframework/$target_framework/$platform_target"
skia_pdf_file_name="skia.pdf"
qpdf_pdf_file_name="qpdf.pdf"
xps_file_name="skia.xps"

rm -rf "$artifacts_root"
rm -rf "$output_dir"
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
(cd "$output_dir" && "$exe" "$platform_target")

"$integration_root/scripts/validate-pdf.sh" "$output_dir/$skia_pdf_file_name"
"$integration_root/scripts/validate-pdf.sh" "$output_dir/$qpdf_pdf_file_name"
"$integration_root/scripts/validate-xps.sh" "$output_dir/$xps_file_name"
