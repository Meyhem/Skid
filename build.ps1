$ErrorActionPreference = "Stop"

$rids = "win-x64", "linux-x64", "linux-arm", "linux-arm64", "osx-x64"
$artifact_folder = "artifacts"


Remove-Item -Force -Recurse -ErrorAction Ignore -Path $artifact_folder
New-Item -ItemType Directory -Force $artifact_folder

$rids | foreach {
    Write-Host "Building $artifact_folder/package-$_"
    dotnet publish --nologo --output "$artifact_folder/package-$_" --configuration Release -r $_ -p:DebugType=None -p:PublishSingleFile=true -p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained true .\Skid\Skid.fsproj
    
    Write-Host "Compressing $artifact_folder/package-$_"
    Compress-Archive -Path $artifact_folder/package-$_/* -DestinationPath $artifact_folder/package-$_.zip
    Remove-Item -Force -Recurse -ErrorAction Ignore -Path $artifact_folder/package-$_
}
