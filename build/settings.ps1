properties {
	$nugetKey = ""

	$script = @{}
	$script.dir = (Split-Path -Parent $MyInvocation.ScriptName)
	
	$base = @{}
	$base.dir = (Split-Path -Parent $script.dir)
	$base.output = (Join-Path $base.dir "Dist")

	$source = @{}
	$source.dir = $base.dir
	$source.solution = @(Get-ChildItem $source.dir -Filter *.sln)[0].Name # "xxx.sln"
	
	$build = @{}
	$build.version = "0.3.3.1"
	$build.configuration = "Release"

	$nuget = @{}
	$nuget.dir = (Join-Path $base.dir ".nuget")
	$nuget.bin = (Join-Path $nuget.dir "nuget.exe")
	$nuget.pushsource = "https://nuget.org/"
	$nuget.sources = @("https://go.microsoft.com/fwlink/?LinkID=206669")
	$nuget.source = @($nuget.sources | ?{ $_ -ne "" -and $_ -ne $null }) -join ";"
	$nuget.output = $base.output
	$nuget.version = "{0}" -f $build.version
}