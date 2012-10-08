param($semverFilename, $assemblyInfoFilename)
Set-StrictMode -Version Latest

# Read revision from git
$version = git describe --tags --long
$matched = $version -match "^v(\d+)\.(\d+)\.(\d+)\-(\d+)-([A-Za-z0-9]{8})$"
$revision = $Matches[4]

# Read semver from file
$content = $( get-content $semverFilename )
$matched = $content -match "^v(\d+)\.(\d+)\.(\d+)$"
($major, $minor, $build) = $Matches[1..3]

# Write to file
$version = "$major.$minor.$build.$revision"

write-host "Building output as $version..."

$assemblyInfo = @"
using System.Reflection;

[assembly: AssemblyVersion("$version")]
[assembly: AssemblyFileVersion("$version")]
"@

$assemblyInfo > $assemblyInfoFilename
