param($assemblyInfoFilename)

$version = git describe --tags --long
$version = $version -replace "v", ""

$matched = $version -match "^(\d)\.(\d)\.(\d)\-(\d)-([A-Za-z0-9]{8})$"

$majorVersion = $Matches[1]
$minorVersion = $Matches[2]
$buildVersion = $Matches[3]
$revisionVersion = $Matches[4]

$version = "$majorVersion.$minorVersion.$buildVersion.$revisionVersion"

write-host "Building output as $version..."

$assemblyVersion = '[assembly: AssemblyVersion("' + $version + '")]'
$assemblyFileVersion = '[assembly: AssemblyFileVersion("' + $version + '")]'


"using System.Reflection;" | out-file $assemblyInfoFilename
"" | out-file $assemblyInfoFilename -append
$assemblyVersion | out-file $assemblyInfoFilename -append
$assemblyFileVersion | out-file $assemblyInfoFilename -append
