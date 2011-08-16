@echo off

:: Microsoft Visual Studio
:: Post-build event command line: call "$(SolutionDir)\PostBuildEvent.bat" "$(ProjectDir)" "$(TargetName)"

:: Created by William Chang
:: Email: william@creativecrew.org
:: Website: http://www.williamchang.org

:: References
:: http://vspug.com/michael/2007/07/05/sharepoint-solution-deployment-handy-post-build-events/

echo.
echo BEGIN Script
echo.
echo Running post-build actions.
echo.

:: BEGIN Debug

:: Example $(ProjectDir) > %ProjectDir%
set ProjectDir=%1
set ProjectDir=%ProjectDir:~1,-2%
echo ProjectDir : %ProjectDir%

:: Example $(TargetName) > %TargetName%
set TargetName=%2
set TargetName=%TargetName:~1,-1%
echo TargetName : %TargetName%

echo.
:: END Debug

:: Run program.
xcopy /y "%ProjectDir%\bin\$(TargetName).*" "C:\inetpub\wwwroot_sandbox_umbraco1\bin"
xcopy /y "%ProjectDir%\masterpages\*.master" "C:\inetpub\wwwroot_sandbox_umbraco1\masterpages"
xcopy /y "%ProjectDir%\usercontrols\*.ascx" "C:\inetpub\wwwroot_sandbox_umbraco1\usercontrols"
xcopy /y "%ProjectDir%\macroScripts\*.cshtml" "C:\Inetpub\wwwroot_sandbox_umbraco1\macroScripts"
xcopy /y "%ProjectDir%\macroScripts\Custom\*.cshtml" "C:\Inetpub\wwwroot_sandbox_umbraco1\macroScripts\Custom"
xcopy /y "%ProjectDir%\usercontrols\Custom\*.ascx" "C:\inetpub\wwwroot_sandbox_umbraco1\usercontrols\Custom"
xcopy /y "%ProjectDir%\usercontrols\Develop\*.ascx" "C:\inetpub\wwwroot_sandbox_umbraco1\usercontrols\Develop"
xcopy /y "%ProjectDir%\umbraco\dialogs\*.aspx" "C:\inetpub\wwwroot_sandbox_umbraco1\umbraco\dialogs"

:end
echo.
echo END Script
echo.