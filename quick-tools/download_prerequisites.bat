@echo off

@REM ###### download C++ WebUI ######

set URL=https://github.com/webui-dev/webui/releases/download/nightly/webui-windows-msvc-x64.zip
set TARGET=QuickTools-BE\QuickTools.Desktop\Native\win-x64\webui

mkdir "%TARGET%" 2>nul
curl -sL "%URL%" -o webui.zip
tar -xf webui.zip
move webui-windows-msvc-x64\* "%TARGET%\" >nul
rmdir /s /q webui-windows-msvc-x64
del webui.zip
echo ✅ Successfully
