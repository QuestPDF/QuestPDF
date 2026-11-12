#!/usr/bin/env bash
#
# Downloads the native libraries built by GitHub Actions and copies them into
# QuestPDF/Runtimes:
#
#   skia   QuestPDF/QuestPDF.Native.Skia.Build   workflow "Build Skia + QuestPDF"
#   qpdf   QuestPDF/QuestPDF.Native.Qpdf.Build   workflow "Build qpdf"
#
# Usage:
#   ./update-native-runtimes.sh             # both libraries, latest successful runs
#   ./update-native-runtimes.sh skia        # only Skia, latest successful run
#   ./update-native-runtimes.sh qpdf        # only qpdf, latest successful run
#   ./update-native-runtimes.sh skia 105    # a specific run number
#
# Files are copied over the existing ones. Anything an artifact does not
# contain is left untouched.
#
# Requires the GitHub CLI (gh), logged in with access to the repositories.

set -euo pipefail

ARTIFACT="questpdf-native-libraries"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARGET="${RUNTIMES_DIR:-$SCRIPT_DIR/QuestPDF/Runtimes}"
LIBRARY="${1:-all}"
RUN_NUMBER="${2:-}"

fail() {
    echo "error: $*" >&2
    exit 1
}

usage() {
    echo "usage: $(basename "$0") [skia|qpdf|all] [run-number]" >&2
    exit 1
}

# update_library <repository> <workflow name> [run number]
update_library() {
    local repo="$1" workflow="$2" run_number="${3:-}"
    local fields='"\(.databaseId) \(.number) \(.headBranch) \(.headSha[0:7]) \(.createdAt)"'
    local run

    if [ -z "$run_number" ]; then
        run=$(gh run list --repo "$repo" --workflow "$workflow" --status success --limit 1 \
            --json databaseId,number,headBranch,headSha,createdAt \
            --jq ".[] | $fields")
        [ -n "$run" ] || fail "no successful '$workflow' run found in $repo"
    else
        run=$(gh run list --repo "$repo" --workflow "$workflow" --limit 200 \
            --json databaseId,number,headBranch,headSha,createdAt \
            --jq ".[] | select(.number == $run_number) | $fields")
        [ -n "$run" ] || fail "'$workflow' run #$run_number not found in $repo"
    fi

    local run_id number branch sha created
    read -r run_id number branch sha created <<< "$run"
    echo "$workflow #$number ($branch @ $sha, $created)"
    echo "https://github.com/$repo/actions/runs/$run_id"

    local download_dir
    download_dir="$(mktemp -d "$DOWNLOAD_ROOT/XXXXXX")"
    gh run download "$run_id" --repo "$repo" --name "$ARTIFACT" --dir "$download_dir"

    local source_dir="$download_dir/runtimes"
    [ -d "$source_dir" ] || fail "the artifact has no 'runtimes' folder"

    local files
    files=$(cd "$source_dir" && find . -type f | sed 's|^\./||' | sort)
    [ -n "$files" ] || fail "the artifact's 'runtimes' folder is empty"

    local file updated=0 unchanged=0
    while IFS= read -r file; do
        if [ -f "$TARGET/$file" ] && cmp -s "$source_dir/$file" "$TARGET/$file"; then
            echo "  unchanged  $file"
            unchanged=$((unchanged + 1))
        else
            mkdir -p "$(dirname "$TARGET/$file")"
            cp "$source_dir/$file" "$TARGET/$file"
            echo "  updated    $file"
            updated=$((updated + 1))
        fi
    done <<< "$files"

    echo "  $updated updated, $unchanged unchanged"
    echo
}

update_skia() { update_library "QuestPDF/QuestPDF.Native.Skia.Build" "Build Skia + QuestPDF" "${1:-}"; }
update_qpdf() { update_library "QuestPDF/QuestPDF.Native.Qpdf.Build" "Build qpdf" "${1:-}"; }

[ $# -le 2 ] || usage
case "$LIBRARY" in
    skia|qpdf|all) ;;
    *) usage ;;
esac

if [ -n "$RUN_NUMBER" ]; then
    [ "$LIBRARY" != "all" ] || fail "a run number needs a library: skia or qpdf"
    [[ "$RUN_NUMBER" =~ ^[0-9]+$ ]] || fail "run number must be a number, got: $RUN_NUMBER"
fi

command -v gh >/dev/null || fail "GitHub CLI (gh) is not installed: https://cli.github.com"
[ -d "$TARGET" ] || fail "target folder not found: $TARGET"

DOWNLOAD_ROOT="$(mktemp -d)"
trap 'rm -rf "$DOWNLOAD_ROOT"' EXIT

echo "Target: $TARGET"
echo

case "$LIBRARY" in
    skia) update_skia "$RUN_NUMBER" ;;
    qpdf) update_qpdf "$RUN_NUMBER" ;;
    all)  update_skia; update_qpdf ;;
esac

echo "Done."
