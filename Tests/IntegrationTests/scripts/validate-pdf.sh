#!/usr/bin/env bash
set -euo pipefail

pdf_path="${1:?PDF path is required}"

if [[ ! -f "$pdf_path" ]]; then
  echo "PDF does not exist: $pdf_path" >&2
  exit 1
fi

size="$(wc -c < "$pdf_path")"

if [[ "$size" -lt 1024 ]]; then
  echo "PDF is too small: $pdf_path ($size bytes)" >&2
  exit 1
fi

header="$(LC_ALL=C head -c 5 "$pdf_path")"

if [[ "$header" != "%PDF-" ]]; then
  echo "PDF header is invalid: $pdf_path" >&2
  exit 1
fi

if ! LC_ALL=C tail -c 2048 "$pdf_path" | grep -q "%%EOF"; then
  echo "PDF EOF marker was not found: $pdf_path" >&2
  exit 1
fi

echo "Validated $pdf_path ($size bytes)"
