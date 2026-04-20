@echo off
set username=QUIDGEST
set password=ZPH2LAB

:: Restore all the NuGet packages to prevent errors in build
dotnet restore

:: Navigate into project folder
cd AdminCLI

:: Build project in Release configuration
dotnet build AdminCLI.csproj -c Release

:: Move into the release folder
cd .\bin\Release\net8.0

:: Reindex the database with default credentials
dotnet AdminCLI.dll reindex -u %username% -p %password%

PAUSE
