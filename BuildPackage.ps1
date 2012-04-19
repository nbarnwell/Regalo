md c:\temp\Packages
gci -filter *.mm.dll -recurse | %{ del $_.fullname }
nuget.exe pack -outputdirectory c:\temp\Packages -symbols