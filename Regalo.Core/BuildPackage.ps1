if (!(test-path c:\temp\Packages)) {
   md c:\temp\Packages
}
nuget.exe pack Regalo.Core.csproj -Build -Symbols -outputdirectory c:\temp\Packages -Properties Configuration=Release