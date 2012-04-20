if (!(test-path c:\temp\Packages)) {
   md c:\temp\Packages
}
nuget.exe pack -Build -Symbols -outputdirectory c:\temp\Packages -Properties Configuration=Release