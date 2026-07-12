; ① Cấu hình chính cho installer
[Setup]

; ② Tên phần mềm hiển thị khi cài đặt
AppName=PC0483 
AppVerName=PC0483

; Phiên bản
; AppVersion=

; Bắt buộc cài vào Program Files 64-bit
ArchitecturesInstallIn64BitMode=x64

; Thư mục cài đặt ứng dụng, {pf} = ProgramFiles
DefaultDirName={pf}\newtun\PC0483

; Tên folder trong Start Menu
DefaultGroupName=PC0483

; Nơi lưu file cài đặt .exe
OutputDir=.

; Tên file cài đặt (.exe)
OutputBaseFilename=PC0483

; Gán icon riêng cho file .exe (⚠️ phải là file .ico)
SetupIconFile=icon.ico

; Thuật toán nén (lzma = tốt, phổ biến)
Compression=lzma

; Nén thành một khối, giảm kích thước file hơn nữa
SolidCompression=yes

; Danh sách file sẽ được đóng gói
[Files]
Source: "..\publish\win64\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs
Source: "icon.ico"; DestDir: "{app}"; Flags: ignoreversion

; Tạo shortcut
[Icons]
; Name: "{group}\PC0483"; \                    ; ⑮ Shortcut trong Start Menu
; Filename: "{app}\PC0483.exe"                 ; ⑯ Trỏ tới file .exe chính
Name: "{group}\PC0483"; IconFilename: "{app}\icon.ico"; Filename: "{app}\PC0483SecretCode.exe";

; Name: "{commondesktop}\MyApp"; \            ; ⑰ Shortcut ngoài desktop
; Filename: "{app}\MyApp.exe"; \
; Tasks: desktopicon                          ; ⑱ Liên kết với task trong phần [Tasks]
Name: "{commondesktop}\PC0483"; IconFilename: "{app}\icon.ico"; Filename: "{app}\PC0483SecretCode.exe"; Tasks: desktopicon;

; Mục lựa chọn thêm trong installer (checkbox)
[Tasks]                                   
; Name: "desktopicon"; \                      ; ⑳ Tên task
; Description: "Create a &desktop icon"; \
; GroupDescription: "Additional icons:"       ; ㉑ Nhóm checkbox sẽ hiển thị
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"