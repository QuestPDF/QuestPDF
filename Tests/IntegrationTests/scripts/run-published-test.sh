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
output_dir="$artifacts_root/output"

case "$app_type" in
  console)
    project_name="QuestPDF.Tests.Console"
    self_contained="false"
    extra_publish_args=()
    ;;
  console-aot)
    project_name="QuestPDF.Tests.Console.Aot"
    self_contained="true"
    extra_publish_args=()
    ;;
  console-singlefile)
    project_name="QuestPDF.Tests.Console.SingleFile"
    self_contained="true"
    extra_publish_args=()
    ;;
  webapi)
    project_name="QuestPDF.Tests.WebApi"
    self_contained="false"
    extra_publish_args=()
    ;;
  worker)
    project_name="QuestPDF.Tests.Worker"
    self_contained="false"
    extra_publish_args=()
    ;;
  *)
    echo "Unknown app type: $app_type" >&2
    exit 2
    ;;
esac

project="$integration_root/$project_name/$project_name.csproj"
rm -rf "$artifacts_root"
mkdir -p "$publish_dir" "$output_dir"

dotnet restore "$project" \
  --configfile "$nuget_config" \
  --framework "$target_framework" \
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
  -p:QuestPDFIntegrationTargetFramework="$target_framework" \
  "${extra_publish_args[@]}"

exe="$publish_dir/$project_name"

if [[ "$rid" == win-* ]]; then
  exe="$exe.exe"
fi

case "$app_type" in
  webapi)
    port="${QUESTPDF_TEST_PORT:-5087}"
    pdf_response="$output_dir/questpdf-webapi-response.pdf"
    log_file="$output_dir/webapi.log"

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

    curl -fsS "http://127.0.0.1:$port/generate-pdf" --output "$pdf_response"
    "$integration_root/scripts/validate-pdf.sh" "$pdf_response"
    ;;
  worker)
    QUESTPDF_TEST_OUTPUT="$output_dir" "$exe"
    "$integration_root/scripts/validate-pdf.sh" "$output_dir/questpdf-integration-worker.pdf"
    ;;
  *)
    "$exe" "$output_dir"
    "$integration_root/scripts/validate-pdf.sh" "$output_dir/questpdf-integration-smoke.pdf"
    ;;
esac
