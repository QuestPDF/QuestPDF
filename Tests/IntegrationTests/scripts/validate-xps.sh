#!/usr/bin/env bash
set -euo pipefail

xps_path="${1:?XPS path is required}"

if [[ ! -f "$xps_path" ]]; then
  echo "Expected XPS file was not created: $xps_path" >&2
  exit 1
fi

size="$(wc -c < "$xps_path" | tr -d '[:space:]')"

if [[ "$size" -lt 1024 ]]; then
  echo "XPS file is too small to be meaningful: $xps_path ($size bytes)" >&2
  exit 1
fi

header="$(LC_ALL=C head -c 4 "$xps_path")"

if [[ "$header" != $'PK\x03\x04' ]]; then
  echo "XPS file does not start with the ZIP package header: $xps_path" >&2
  exit 1
fi
