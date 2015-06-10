call "c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VSVARS32.bat"

msbuild ..\src\TestConsole.sln /t:clean
msbuild ..\src\TestConsole.sln /t:Rebuild /p:Configuration=Release

if NOT EXIST "TestConsole %testconsoleversion%" md "TestConsole %testconsoleversion%"

copy ..\src\TestConsole\bin\release\*.dll "TestConsole %testconsoleversion%"
copy ..\src\TestConsole\bin\release\*.xml "TestConsole %testconsoleversion%"
copy ..\src\TestConsole\bin\release\*.dll "TestConsole_current"
copy ..\src\TestConsole\bin\release\*.xml "TestConsole_current"

nuget pack ..\src\TestConsole\TestConsole.csproj -outputdirectory "TestConsole_current" -IncludeReferencedProjects -Prop Configuration=Release

copy "TestConsole_current\*.nupkg" "TestConsole %testconsoleversion%"

git\git add "TestConsole %testconsoleversion%\*.dll" -f
git\git add "TestConsole %testconsoleversion%\*.xml" -f
git\git add "TestConsole %testconsoleversion%\*.nupkg" -f
git\git add "TestConsole_current\*.dll" -f
git\git add "TestConsole_current\*.xml" -f
git\git add "TestConsole_current\*.nupkg" -f
pause