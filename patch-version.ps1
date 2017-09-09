param($projectPath, $version, $packageVersion)

Write-Host "Patching with version $version"

$csprojPath = Join-Path $PSScriptRoot $projectPath
[xml]$project = Get-Content -Path $csprojPath
$project.Project.PropertyGroup[0].Version = $version
$project.Project.PropertyGroup[0].PackageVersion = $packageVersion
$project.Save($csprojPath)