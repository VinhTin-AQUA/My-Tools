#!/bin/bash
set -e

export ANDROID_NDK_HOME=$HOME/Android/Sdk
export ANDROID_NDK_ROOT=$ANDROID_HOME/ndk/27.3.13750724

export CMAKE=cmake
export CC=$NDK_ROOT/toolchains/llvm/prebuilt/linux-x86_64/bin/x86_64-linux-android21-clang
export CXX=$NDK_ROOT/toolchains/llvm/prebuilt/linux-x86_64/bin/x86_64-linux-android21-clang++

export PATH="$PATH:$ANDROID_HOME/platform-tools:$ANDROID_HOME/emulator:$ANDROID_HOME/cmdline-tools/latest/bin"

export BINDGEN_EXTRA_CLANG_ARGS="--sysroot=$NDK_ROOT/toolchains/llvm/prebuilt/linux-x86_64/sysroot -I$NDK_ROOT/toolchains/llvm/prebuilt/linux-x86_64/sysroot/usr/include"

export OPENSSL_DIR=/usr/bin/openssl
export PKG_CONFIG_PATH=/usr/bin/pkg-config

echo "✅ Environment ready. Running Tauri Android Dev..."
# npm run tauri android dev

PID=$(sudo lsof -t -i:4200) && if [ -n "$PID" ]; then sudo kill -9 $PID && echo "Killed process $PID using port 4200."; else echo "No process using port 4200."; fi; ng serve --host 0.0.0.0 --port 4200 & npm run tauri android dev