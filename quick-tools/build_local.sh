#!/bin/bash

set -e

# Always use the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PUBLISH_FOLDER_WINDOWS="quicktools-windows"
PUBLISH_FOLDER_LINUX="quicktools-linux"

echo "========================================"
echo "       BUILD QUICKTOOLS"
echo "========================================"
echo
echo "Script directory: $SCRIPT_DIR"
echo


# ================================
# 1. Build Angular frontend
# ================================
echo "[1/4] Installing frontend packages..."

cd "$SCRIPT_DIR/quicktools-fe"


echo
echo "[2/4] Building Angular frontend..."

ng build --configuration=production


# ================================
# 2. Copy frontend to wwwroot
# ================================
echo
echo "[3/4] Copying frontend files..."

FRONTEND_DIST="$SCRIPT_DIR/quicktools-fe/dist/quicktools-fe/browser"
WWWROOT="$SCRIPT_DIR/QuickTools-BE/QuickTools.Desktop/wwwroot"

echo "Frontend dist: $FRONTEND_DIST"
echo "Target wwwroot: $WWWROOT"

if [ ! -d "$FRONTEND_DIST" ]; then
    echo "ERROR: Frontend build output not found:"
    echo "$FRONTEND_DIST"
    exit 1
fi

mkdir -p "$WWWROOT"

cp -R "$FRONTEND_DIST/." "$WWWROOT/"

