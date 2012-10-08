$scriptDir = $MyInvocation.MyCommand.Path | split-path
$outputDir = "$scriptDir\BuildOutput"

md $outputDir -f

gci $scriptDir -include Regalo.Core.csproj,Regalo.RavenDB.csproj,Regalo.Testing.csproj -recurse | %{ 
    nuget.exe pack $_ -Build -Symbols -outputdirectory $outputDir -Properties Configuration=Release
}