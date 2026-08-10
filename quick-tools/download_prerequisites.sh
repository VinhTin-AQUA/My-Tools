#!/bin/bash


##################### Download C++ WebUI #####################

URL="https://github.com/webui-dev/webui/releases/download/nightly/webui-linux-gcc-x64.zip"
TARGET="QuickTools-BE/QuickTools.Desktop/Native/linux-x64/webui"

mkdir -p "$TARGET"
curl -sL "$URL" -o webui.zip
unzip -q webui.zip
mv webui-linux-gcc-x64/* "$TARGET/"
rm -rf webui-linux-gcc-x64 webui.zip
echo "✅ Successfully"

