export KEYSTORE_PATH="newtun-release-key.keystore" # newtun-release-key.keystore lưu tại src-tauri/gen/android/app/newtun-release-key.keystore
export STORE_PASSWORD="abcdef@123"
export KEY_ALIAS="newtunkey"
export KEY_PASSWORD="abcdef@123"

export JAVA_HOME=/home/newtun/.local/apps/android-studio/jbr
export ANDROID_HOME=$HOME/Android/Sdk
export ANDROID_NDK_ROOT=$ANDROID_HOME/ndk/27.3.13750724

npm run tauri android build -- --target aarch64