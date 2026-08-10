#!/bin/bash

set -e

# Always use the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

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

npm install


echo
echo "[2/4] Building Angular frontend..."

ng build


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


# ================================
# 3. Clean publish folder
# ================================
echo
echo "Cleaning publish folder..."

if [ -d "$SCRIPT_DIR/publish" ]; then
    rm -rf "$SCRIPT_DIR/publish"
fi


# ================================
# 4. Dotnet publish
# ================================
echo
echo "[4/4] Publishing .NET application..."

dotnet restore "$SCRIPT_DIR/QuickTools-BE/QuickTools.Desktop/QuickTools.Desktop.csproj"

dotnet publish \
    "$SCRIPT_DIR/QuickTools-BE/QuickTools.Desktop/QuickTools.Desktop.csproj" \
    -c Release \
    -r linux-x64 \
    -p:SelfContained=true \
    -o "$SCRIPT_DIR/publish"


# ================================
# Rename EXE
# ================================
echo
echo "Renaming executable..."

if [ -f "$SCRIPT_DIR/publish/QuickTools.Desktop" ]; then
    mv \
        "$SCRIPT_DIR/publish/QuickTools.Desktop" \
        "$SCRIPT_DIR/publish/QuickTools"
else
    echo "ERROR: QuickTools.Desktop not found."
    exit 1
fi


echo
echo "========================================"
echo "         BUILD SUCCESS"
echo "========================================"
echo
echo "Output:"
echo "$SCRIPT_DIR/publish"
echo
