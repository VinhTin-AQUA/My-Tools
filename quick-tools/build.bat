@echo off
setlocal

echo ========================================
echo        BUILD QUICKTOOLS
echo ========================================
echo.

REM ================================
REM 1. Build Angular frontend
REM ================================
echo [1/4] Installing frontend packages...

cd /d "%~dp0quicktools-fe"

call npm install
if errorlevel 1 (
    echo ERROR: npm install failed.
    exit /b 1
)

echo.
echo [2/4] Building Angular frontend...

call ng build --configuration=production
if errorlevel 1 (
    echo ERROR: ng build failed.
    exit /b 1
)

cd /d "%~dp0"


REM ================================
REM 2. Copy frontend to wwwroot
REM ================================
echo.
echo [3/4] Copying frontend files...

xcopy "quicktools-fe\dist\quicktools-fe\browser\*" ^
      "QuickTools-BE\QuickTools.Windows\wwwroot\" ^
      /E /I /Y

if errorlevel 1 (
    echo ERROR: Copy frontend files failed.
    exit /b 1
)


REM ================================
REM 3. Clean publish folder
REM ================================
echo.
echo Cleaning publish folder...

if exist "publish" (
    rmdir /S /Q "publish"
)


REM ================================
REM 4. Dotnet publish
REM ================================
echo.
echo [4/4] Publishing .NET application...

dotnet restore "QuickTools-BE\QuickTools.Desktop\QuickTools.Desktop.csproj"

dotnet publish ^
    "QuickTools-BE\QuickTools.Desktop\QuickTools.Desktop.csproj" ^
    -c Release ^
    -r win-x64 ^
    -p:SelfContained=true ^
    -o ".\publish"

if errorlevel 1 (
    echo ERROR: dotnet publish failed.
    exit /b 1
)


REM ================================
REM Rename EXE
REM ================================
echo.
echo Renaming executable...

if exist "publish\QuickTools.Desktop.exe" (
    ren "publish\QuickTools.Desktop.exe" "QuickTools.exe"
) else (
    echo ERROR: QuickTools.Desktop.exe not found.
    exit /b 1
)


echo.
echo ========================================
echo          BUILD SUCCESS
echo ========================================
echo.
echo Output:
echo %~dp0publish
echo.


