@Title ɾ��HighlightingSystem��
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo ��ȷ���Ƿ�ɾ��HighlightingSystem��?
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

rmdir /q /s %cd%\HighlightingSystemDemo
del /f /q /s HighlightingSystemDemo.meta
del /f /q /s /ah HighlightingSystemDemo.meta

cd ./Plugins

rmdir /q /s %cd%\HighlightingSystem
del /f /q HighlightingSystem.meta
del /f /q /ah HighlightingSystem.meta


exit