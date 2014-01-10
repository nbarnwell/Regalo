$scriptDir = $MyInvocation.MyCommand.Path | split-path
$outputDir = "$scriptDir\BuildOutput"

md $outputDir -f | out-null

gci $scriptDir -include Regalo.Core.csproj,Regalo.RavenDB.csproj,Regalo.SqlServer.csproj,Regalo.Testing.csproj,Regalo.ObjectCompare.csproj -recurse | %{ 
    Write-Host -ForegroundColor Green "$_"
    nuget.exe pack $_ -Build -Symbols -outputdirectory $outputDir -Properties Configuration=Release
}