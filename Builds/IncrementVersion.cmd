set /p Increment=Really increment the revision number for TestConsole? Enter Y to proceed: 
if %Increment%==Y tools\setcsprojver .. /i:fv,b /env:testconsoleversion
if %Increment%==Y tools\setcsprojver .. /i:av,b /env:testconsoleversion

pause