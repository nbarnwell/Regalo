param($projectName, $assemblyInfoFilename)
Set-StrictMode -Version Latest

$gitVersion = git describe --tags --long --match "$projectName-v*.*.*" --abbrev=40
$gitVersion -match "^$projectName-v(\d+)\.(\d+)\.(\d+)\-(\d+)-(g[a-f0-9]{40})`$"
($major, $minor, $build, $revision) = $Matches[1..4]

$assemblyVersion = "$major.$minor.$build.$revision"

write-host "Building output as $assemblyVersion..."

$assemblyInfo = @"
using System.Reflection;

[assembly: AssemblyDescription("A basic implementation of Greg Young's CQRS/ES pattern. Built from version $gitVersion.")]
[assembly: AssemblyVersion("$assemblyVersion")]
[assembly: AssemblyFileVersion("$assemblyVersion")]
"@

$assemblyInfo > $assemblyInfoFilename
