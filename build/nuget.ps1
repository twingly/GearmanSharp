
Task Nuget-Init {
	New-Item $nuget.output -Type directory -Force | Out-Null
}

Task Nuget-Pack -Depends Nuget-Init {
	Get-ChildItem $source.dir -Recurse -Filter *.nuspec |
		%{
			if ($nuget.nuspec_pack -contains ([System.IO.Path]::GetFileNameWithoutExtension($_.FullName))) {
				# when project folder name is in nuspec_pack list, use nuget path
				$_.FullName
			} else {
				# where project isn't in nuspec_pack list, use the csproj path instead
				Join-Path (Split-Path -Parent $_.FullName) ([System.IO.Path]::ChangeExtension((Split-Path -Leaf $_.FullName), ".csproj"))
			}
		} |
		%{ Nuget-Pack $_ }
}

function Nuget-Pack {
	[CmdletBinding()]
	param(
		[Parameter(Position=0,Mandatory=1)]$file
	)
	
	Write-Output "Packing $file"
	
	exec { & $nuget.bin pack $file -Properties "Configuration=$($build.configuration)" -OutputDirectory $nuget.output -BasePath (Split-Path -Parent $file) -Version $nuget.version }
}

Task Nuget-Push -Depends Nuget-Pack {
	Get-ChildItem $nuget.output -Filter "*$($nuget.version).nupkg" |
		%{ Nuget-Push $_.FullName }
}

function Nuget-Push {
	[CmdletBinding()]
	param(
		[Parameter(Position=0,Mandatory=1)]$file
	)
	
	Write-Output "Pushing $file key: $nugetKey"
	#exec { & $nuget.bin push $file $nugetKey -Source $nuget.pushsource }
}