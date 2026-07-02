#!/usr/bin/env bash
set -euo pipefail

app_type="${1:?app type is required}"
target_framework="${2:?target framework is required}"
rid="${3:?runtime identifier is required}"
package_version="${4:?QuestPDF package version is required}"

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
integration_root="$repo_root/Tests/IntegrationTests"
nuget_config="$integration_root/nuget.config"
artifacts_root="$repo_root/artifacts/integration/$app_type/$target_framework/$rid"
publish_dir="$artifacts_root/publish"
output_dir="$repo_root/artifacts/integration-output/$app_type/$target_framework/$rid"
log_dir="$artifacts_root/logs"
skia_pdf_file_name="questpdf-integration-$app_type-$target_framework-$rid-skia.pdf"
qpdf_pdf_file_name="questpdf-integration-$app_type-$target_framework-$rid-qpdf.pdf"
xps_file_name="questpdf-integration-$app_type-$target_framework-$rid-skia.xps"

case "$app_type" in
  console)
    project_name="QuestPDF.Tests.Console"
    self_contained="false"
    ;;
  console-aot)
    project_name="QuestPDF.Tests.Console.Aot"
    self_contained="true"
    ;;
  console-singlefile)
    project_name="QuestPDF.Tests.Console.SingleFile"
    self_contained="true"
    ;;
  webapi)
    project_name="QuestPDF.Tests.WebApi"
    self_contained="false"
    ;;
  worker)
    project_name="QuestPDF.Tests.Worker"
    self_contained="false"
    ;;
  *)
    echo "Unknown app type: $app_type" >&2
    exit 2
    ;;
esac

project="$integration_root/$project_name/$project_name.csproj"
rm -rf "$artifacts_root"
rm -rf "$output_dir"
mkdir -p "$publish_dir" "$output_dir" "$log_dir"

dotnet restore "$project" \
  --configfile "$nuget_config" \
  --runtime "$rid" \
  -p:QuestPDFIntegrationVersion="$package_version" \
  -p:QuestPDFIntegrationTargetFramework="$target_framework"

dotnet publish "$project" \
  --configuration Release \
  --framework "$target_framework" \
  --runtime "$rid" \
  --self-contained "$self_contained" \
  --output "$publish_dir" \
  --no-restore \
  -p:QuestPDFIntegrationVersion="$package_version" \
  -p:QuestPDFIntegrationTargetFramework="$target_framework"

exe="$publish_dir/$project_name"

if [[ "$rid" == win-* ]]; then
  exe="$exe.exe"
fi

case "$app_type" in
  webapi)
    port="${QUESTPDF_TEST_PORT:-5087}"
    skia_pdf_response="$output_dir/$skia_pdf_file_name"
    qpdf_pdf_response="$output_dir/$qpdf_pdf_file_name"
    xps_response="$output_dir/$xps_file_name"
    log_file="$log_dir/webapi.log"

    ASPNETCORE_URLS="http://127.0.0.1:$port" "$exe" > "$log_file" 2>&1 &
    server_pid=$!

    cleanup() {
      if kill -0 "$server_pid" >/dev/null 2>&1; then
        kill "$server_pid" >/dev/null 2>&1 || true
        wait "$server_pid" >/dev/null 2>&1 || true
      fi
    }

    trap cleanup EXIT

    for _ in $(seq 1 60); do
      if curl -fsS "http://127.0.0.1:$port/health" >/dev/null 2>&1; then
        break
      fi

      if ! kill -0 "$server_pid" >/dev/null 2>&1; then
        cat "$log_file" >&2
        exit 1
      fi

      sleep 1
    done

    curl -fsS "http://127.0.0.1:$port/generate-skia-pdf" --output "$skia_pdf_response"
    "$integration_root/scripts/validate-pdf.sh" "$skia_pdf_response"

    curl -fsS "http://127.0.0.1:$port/generate-pdf" --output "$qpdf_pdf_response"
    "$integration_root/scripts/validate-pdf.sh" "$qpdf_pdf_response"

    if [[ "$rid" == win-* ]]; then
      curl -fsS "http://127.0.0.1:$port/generate-xps" --output "$xps_response"
      "$integration_root/scripts/validate-xps.sh" "$xps_response"
    fi
    ;;
  worker)
    if [[ "$rid" == win-* ]]; then
      QUESTPDF_TEST_OUTPUT="$output_dir" QUESTPDF_TEST_SKIA_PDF_FILE="$skia_pdf_file_name" QUESTPDF_TEST_QPDF_PDF_FILE="$qpdf_pdf_file_name" QUESTPDF_TEST_XPS_FILE="$xps_file_name" "$exe"
    else
      QUESTPDF_TEST_OUTPUT="$output_dir" QUESTPDF_TEST_SKIA_PDF_FILE="$skia_pdf_file_name" QUESTPDF_TEST_QPDF_PDF_FILE="$qpdf_pdf_file_name" "$exe"
    fi

    "$integration_root/scripts/validate-pdf.sh" "$output_dir/$skia_pdf_file_name"
    "$integration_root/scripts/validate-pdf.sh" "$output_dir/$qpdf_pdf_file_name"

    if [[ "$rid" == win-* ]]; then
      "$integration_root/scripts/validate-xps.sh" "$output_dir/$xps_file_name"
    fi
    ;;
  *)
    if [[ "$rid" == win-* ]]; then
      "$exe" "$output_dir" "$skia_pdf_file_name" "$qpdf_pdf_file_name" "$xps_file_name"
    else
      "$exe" "$output_dir" "$skia_pdf_file_name" "$qpdf_pdf_file_name"
    fi

    "$integration_root/scripts/validate-pdf.sh" "$output_dir/$skia_pdf_file_name"
    "$integration_root/scripts/validate-pdf.sh" "$output_dir/$qpdf_pdf_file_name"

    if [[ "$rid" == win-* ]]; then
      "$integration_root/scripts/validate-xps.sh" "$output_dir/$xps_file_name"
    fi
    ;;
esac
