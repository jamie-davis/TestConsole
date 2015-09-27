call "c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VSVARS32.bat"

msbuild ..\src\TestConsole.sln /t:clean /p:Configuration=Debug
msbuild ..\src\TestConsole.sln /t:Rebuild /p:Configuration=Debug

if EXIST "Patch" rd /s/q "Patch"
md "Patch"

copy ..\src\TestConsole\bin\release\*.dll "Patch"
copy ..\src\TestConsole\bin\release\*.xml "Patch"
copy ..\src\TestConsole\bin\release\*.pdb "Patch"

.nuget\nuget pack ..\src\TestConsole\TestConsole.csproj -symbols -outputdirectory "Patch" -IncludeReferencedProjects -Prop Configuration=Debug -Version %testconsoleversion%

pause