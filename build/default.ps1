$path = (Split-Path -Parent $MyInvocation.MyCommand.path)

include (Join-Path $path "settings.ps1")
include (Join-Path $path "msbuild.ps1")
include (Join-Path $path "nuget.ps1")

Task Default -depends Compile, Nuget-Push

Task Compile -depends Invoke-MSBuild