set /p Increment=Really increment the revision number for TestConsole? Enter Y to proceed: 
if %Increment%==Y (
    tools\setcsprojver .. /i:fv,r /env:testconsoleversion
    tools\setcsprojver .. /i:av,r /env:testconsoleversion
)

pause