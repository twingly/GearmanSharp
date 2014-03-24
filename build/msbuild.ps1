Task Invoke-MSBuild {
	exec { msbuild (Resolve-Path "$($source.dir)\$($source.solution)") /p:Configuration=$($build.configuration) /p:RestorePackages=false }
}