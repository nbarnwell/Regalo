$scriptDir = $MyInvocation.MyCommand.Path | split-path
$outputDir = "$scriptDir\BuildOutput"

md $outputDir -f

gci $scriptDir -include Regalo.Core.csproj,Regalo.RavenDB.csproj -recurse | %{ 
    nuget.exe pack $_ -Build -Symbols -outputdirectory $outputDir -Properties Configuration=Release
}