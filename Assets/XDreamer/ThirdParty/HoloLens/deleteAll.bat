@Title ɾ��HoloLens��
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo ��ȷ���Ƿ�ɾ��HoloLens��?
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

% ---assets��Ŀ¼--- %

cd ./../../../

rmdir /q /s %cd%\HoloToolkit
del /f /q /s HoloToolkit.meta
del /f /q /s /ah HoloToolkit.meta

rmdir /q /s %cd%\HoloToolkit-Examples
del /f /q /s HoloToolkit-Examples.meta
del /f /q /s /ah HoloToolkit-Examples.meta

rmdir /q /s %cd%\HoloToolkit-Preview
del /f /q /s HoloToolkit-Preview.meta
del /f /q /s /ah HoloToolkit-Preview.meta

exit