#!/bin/bash

cd Thanh-Video-Labeler-App
npm ci
npm run build -- --configuration production

cd ..

rm -rf Thanh-Video-Labeler_publish_linux
rm -rf Thanh-Video-Labeler_publish_win

cp -r Thanh-Video-Labeler-App/dist/ExcelVideoLabelerApp/browser/* Thanh-Video-Labeler/wwwroot/

dotnet publish Thanh-Video-Labeler/Thanh-Video-Labeler.csproj -c Release -r linux-x64 -p:SelfContained=true  -o ./Thanh-Video-Labeler_publish_linux
dotnet publish Thanh-Video-Labeler/Thanh-Video-Labeler.csproj -c Release -r win-x64 -p:SelfContained=true  -o ./Thanh-Video-Labeler_publish_win

mv Thanh-Video-Labeler_publish_linux/Thanh-Video-Labeler Thanh-Video-Labeler_publish_linux/app
mv Thanh-Video-Labeler_publish_win/Thanh-Video-Labeler.exe Thanh-Video-Labeler_publish_win/app.exe
