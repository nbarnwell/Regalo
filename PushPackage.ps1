write-verbose "Preparing to push packages to nuget..."

$scriptDir = $MyInvocation.MyCommand.Path | split-path
$outputDir = "$scriptDir\BuildOutput"
write-debug "outputDir = $outputDir"

md $outputDir -f | out-null

gci $outputDir -filter *.nupkg | %{ 
    Write-Host -ForegroundColor Green "$_"
    nuget.exe push $_.FullName
}