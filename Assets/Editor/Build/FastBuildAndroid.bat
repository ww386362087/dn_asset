@echo off

::-----------Config-----------::
set UNITY_PATH=D:\install\Unity5\Editor\Unity.exe
set PROJECT_PATH=D:\projects\dn_asset
::-----------Config-----------::


echo ****** Unity to apk *******

echo %PROJECT_PATH%

"%UNITY_PATH%" -projectPath "%PROJECT_PATH%" -executeMethod XBuild.FastBuildAndroid -quit -batchmode

echo ****** build finish ********

pause