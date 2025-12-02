@echo off
setlocal
set CONFIG=Release
set TFM=net48
set PROJECT_NAME=PhotoMode
set OUT_DIR=bin\%CONFIG%\%TFM%
set TARGET_DLL=PhotoMode.dll
set MOD_DEST=C:\Users\andre\OneDrive\Desktop\big folder\tester builds\VAP_HalloweenBuildv1\VAP_HalloweenBuildv1\BepInEx\plugins
dotnet build "%PROJECT_NAME%.csproj" -c %CONFIG%
if %errorlevel% neq 0 exit /b 1
if not exist "%MOD_DEST%" mkdir "%MOD_DEST%"
copy "%OUT_DIR%\%TARGET_DLL%" "%MOD_DEST%\" /Y
