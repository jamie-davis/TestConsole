call "c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VSVARS32.bat"

msbuild ..\src\TestConsole.sln /t:clean
msbuild ..\src\TestConsole.sln /t:Rebuild /p:Configuration=Release

if NOT EXIST "TestConsole %testconsoleversion%" md "TestConsole %testconsoleversion%"

copy ..\src\TestConsole\bin\release\*.dll "TestConsole %testconsoleversion%"
copy ..\src\TestConsole\bin\release\*.xml "TestConsole %testconsoleversion%"

.nuget\nuget pack ..\src\TestConsole\TestConsole.csproj -outputdirectory "TestConsole %testconsoleversion%" -IncludeReferencedProjects -Prop Configuration=Release -Version %testconsoleversion%

git add "TestConsole %testconsoleversion%\TestConsole.dll" -f
git add "TestConsole %testconsoleversion%\TestConsole.xml" -f
git add "TestConsole %testconsoleversion%\TestConsole.%testconsoleversion%.nupkg" -f
pause