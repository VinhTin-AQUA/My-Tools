# for /f "tokens=5" %a in ('netstat -ano ^| findstr :4200') do taskkill /PID %a /F # for windows
sudo kill -9 $(sudo lsof -t -i:4200) # for linux
npm run tauri dev