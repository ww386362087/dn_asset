@echo off

::-----------Config-----------::
set UNITY_PATH=D:\install\Unity5\Editor\Unity.exe
set PROJECT_PATH=D:\projects\dn_asset
::-----------Config-----------::


echo ******** Unity to Win32 ********

echo ProjectPath:%PROJECT_PATH%

"%UNITY_PATH%" -projectPath "%PROJECT_PATH%" -executeMethod XBuild.BuildWin32 -quit -batchmode

echo *********** Build finish *********

pause