@echo off
echo Cleaning and rebuilding VaultScope WPF project...
echo.

cd /d "%~dp0\VaultScope.Enterprise.WPF"

echo Step 1: Cleaning bin and obj directories...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
echo Clean complete.
echo.

echo Step 2: Restoring NuGet packages...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to restore NuGet packages
    pause
    exit /b 1
)
echo.

echo Step 3: Building project in Release mode...
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)
echo.

echo Build completed successfully!
echo.
echo The application is ready to run from:
echo %cd%\bin\Release\net8.0-windows\VaultScope.Enterprise.WPF.exe
echo.
pause