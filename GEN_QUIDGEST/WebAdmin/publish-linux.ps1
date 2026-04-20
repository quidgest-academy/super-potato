$vs = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -products * -requires Microsoft.Component.MSBuild  -version "[17.8,)" -prerelease -latest -utf8 -format json | ConvertFrom-Json
$msbuild = Join-Path -Path $vs[0].installationPath -ChildPath "MSBuild\Current\Bin\MSBuild.exe"
& $msbuild ./Administration/Administration.csproj -restore /p:DeployOnBuild=true /p:PublishProfile=linux
& $msbuild ./AdminCLI/AdminCLI.csproj /t:Publish -restore /p:DeployOnBuild=true /p:PublishProfile=linux /p:Configuration=Release