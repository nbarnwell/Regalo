$outputDir = "$scriptDir\BuildOutput"

md $outputDir -f | out-null

gci $outputDir -include *.nupkg | %{ 
    Write-Host -ForegroundColor Green "$_"
    nuget.exe push $_
}