dotnet publish --output package --configuration Release -r win-x64 -p:DebugType=None -p:PublishSingleFile=true -p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained true .\Skid\Skid.fsproj