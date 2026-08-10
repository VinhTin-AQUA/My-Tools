#!/bin/bash


##################### Download C++ WebUI Linux #####################

URL="https://github.com/webui-dev/webui/releases/download/nightly/webui-linux-gcc-x64.zip"
TARGET="QuickTools-BE/QuickTools.Desktop/Native/linux-x64/webui"

mkdir -p "$TARGET"
curl -sL "$URL" -o webui-linux-x64.zip
unzip -q webui-linux-x64.zip
mv webui-linux-gcc-x64/* "$TARGET/"
rm -rf webui-linux-gcc-x64 webui-linux-x64.zip
echo "✅ Successfully"

##################### Download C++ WebUI Windows #####################

URL="https://github.com/webui-dev/webui/releases/download/nightly/webui-windows-msvc-x64.zip"
TARGET="QuickTools-BE/QuickTools.Desktop/Native/win-x64/webui"

mkdir -p "$TARGET"
curl -sL "$URL" -o webui-win-x64.zip
unzip -q webui-win-x64.zip
mv webui-windows-msvc-x64/* "$TARGET/"
rm -rf webui-windows-msvc-x64 webui-win-x64.zip
echo "✅ Successfully"


