export ANDROID_NDK_HOME=/home/newtun/Android/Sdk/ndk/27.3.13750724
export ANDROID_NDK_ROOT=/home/newtun/Android/Sdk/ndk/27.3.13750724
export ANDROID_NDK=$ANDROID_NDK_HOME
echo $ANDROID_NDK_HOME

sudo kill -9 $(sudo lsof -t -i:4200) # for linux
ng serve --host 0.0.0.0 --port 4200 & npm run tauri android dev