@Title ɾ��SteamVR��
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo ��ȷ���Ƿ�ɾ��SteamVR��?
)else (
	if "%one%"=="Y" (
		goto :excute
	)
	if "%one%"=="y" (
		goto :excute
	)else (
		exit
	)
)

set /p c=������ָ��(Y/N):
set one=%c%
goto :condition

:excute

set currentDirectory=%cd%

pushd %cd%

cd ./../../../

rmdir /q /s %cd%\SteamVR
del /f /q SteamVR.meta
del /f /q /ah SteamVR.meta

;�����Ƴ���ǰĿ¼ del /f /q /s %currentDirectory%.meta
;�����Ƴ���ǰĿ¼ del /f /q /s /ah %currentDirectory%.meta
;�����Ƴ���ǰĿ¼ rmdir /q /s %currentDirectory%

exit