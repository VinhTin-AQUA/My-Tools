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



# ================================
# 3. Clean publish folder
# ================================
echo
echo "Cleaning publish folder..."

if [ -d "$SCRIPT_DIR/$PUBLISH_FOLDER_LINUX" ]; then
    rm -rf "$SCRIPT_DIR/$PUBLISH_FOLDER_LINUX"
fi

if [ -d "$SCRIPT_DIR/$PUBLISH_FOLDER_WINDOWS" ]; then
    rm -rf "$SCRIPT_DIR/$PUBLISH_FOLDER_WINDOWS"
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
    -o "$SCRIPT_DIR/$PUBLISH_FOLDER_LINUX"

dotnet publish \
    "$SCRIPT_DIR/QuickTools-BE/QuickTools.Desktop/QuickTools.Desktop.csproj" \
    -c Release \
    -r win-x64 \
    -p:SelfContained=true \
    -o "$SCRIPT_DIR/$PUBLISH_FOLDER_WINDOWS"

# ================================
# Rename EXE
# ================================
echo
echo "Renaming executable..."

if [ -f "$SCRIPT_DIR/$PUBLISH_FOLDER_LINUX/QuickTools.Desktop" ]; then
    mv \
        "$SCRIPT_DIR/$PUBLISH_FOLDER_LINUX/QuickTools.Desktop" \
        "$SCRIPT_DIR/$PUBLISH_FOLDER_LINUX/QuickTools"
else
    echo "ERROR: QuickTools.Desktop not found."
    exit 1
fi

if [ -f "$SCRIPT_DIR/$PUBLISH_FOLDER_WINDOWS/QuickTools.Desktop.exe" ]; then
    mv \
        "$SCRIPT_DIR/$PUBLISH_FOLDER_WINDOWS/QuickTools.Desktop.exe" \
        "$SCRIPT_DIR/$PUBLISH_FOLDER_WINDOWS/QuickTools.exe"
else
    echo "ERROR: QuickTools.Desktop.exe not found."
    exit 1
fi
