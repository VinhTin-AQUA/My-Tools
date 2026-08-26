# QuickTools

## Setup, run and build

### Desktop

- Setup: run **download_prerequisites.sh (Linux)** or **download_prerequisites.bat (Windows)**
- Run:
    - Check native folder **QuickTools-BE/QuickTools.Desktop/Native/linux-x64** or **QuickTools-BE/QuickTools.Desktop/Native/win-x64** same with **[Native folder structure](#native-folder-structure)**
    - Open project by Visual Studio or Rider and Run

- Build: after run download_prerequisites script, run **build.sh (linux)** or **build.bat (windows)**

### Android


## Native folder structure

- Cấu trúc thư mục Native

```txt
Native/
├── linux-x64/
│   ├── ffmpeg/
│   │   ├── libavutil.so
│   │   ├── libswresample.so
│   │   ├── libswscale.so
│   │   ├── libavcodec.so
│   │   ├── libavformat.so
│   │   ├── libavfilter.so
│   │   └── libavdevice.so
│   └── webui/
├── win-x64/
│   ├── ffmpeg/
│   │   ├── avutil.dll
│   │   ├── swresample.dll
│   │   ├── swscale.dll
│   │   ├── avcodec.dll
│   │   ├── avformat.dll
│   │   ├── avfilter.dll
│   │   └── avdevice.dll
│   └── webui/
├── linux-arm64/
└── win-arm64/
```

- Tên thư mục phải trùng với tên trong DllImport. Ví dụ "webui"

## Load binaries

- Vào QuickTools/Modules/LoaderManager/NativeLibraryManager.cs, thêm thư mục thư viện

```cs
private static void RegisterLibraries()
{
    // Đăng ký từng thư viện
    RegisterLibrary("webui", "webui");
    RegisterLibrary("ffmpeg", "ffmpeg");
    // Thêm các thư viện khác nếu cần
    // RegisterLibrary("opencv", "opencv");
    // RegisterLibrary("tensorflow", "tensorflow");
}
```

## build

```txt
dotnet publish QuickTools\QuickTools.csproj -c Release -r win-x64 -p:SelfContained=true  -o ./publish

dotnet publish QuickTools/QuickTools.csproj -c Release -r linux-x64 -p:SelfContained=true  -o ./publish
```

```cs
private const string Library = "webui";
```

## Prompts

- UI

``txt
trong net blazor, sử dụng html tailwind và css để code giao diện responsive, đẹp, hiện đại như sau:

giao diện gồm 2 phần:
phần đầu chứa các thông tin sau: 

public IP, Autonomous System, ISP, City, các dữ liệu sử dụng dữ liệu tạm thời

phần thứ 2 là danh sách các menu: gồm có 1 thanh search menu tự động, bên dưới là danh sách menu ở dạng grid, mỗi menu item gồm icon và tên menu






file css thuần thì chỉ được style màu sắc cho theme sử dụng các biến màu được định nghĩa bên dưới, 
còn các stlye như layout, size, font,... thì phải sử dụng class tailwind inline html

:root {
    --color-primary: #2563EB;
    --color-primary-hover: #1D4ED8;

    --color-secondary: #64748B;

    --color-background: #F8FAFC;
    --color-surface: #FFFFFF;

    --color-text-primary: #0F172A;
    --color-text-secondary: #64748B;
    --color-text-disabled: #94A3B8;

    --color-border: #E2E8F0;

    --color-success: #16A34A;
    --color-warning: #D97706;
    --color-error: #DC2626;
    --color-info: #0284C7;
}
```