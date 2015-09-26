@echo off
echo Patch files:
echo.
dir patch\*.nupkg /B
echo.
echo About to copy patch version to local nuget (..\..\NugetLocal)
pause 
copy Patch\*.nupkg ..\..\NugetLocal