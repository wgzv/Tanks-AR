@Title ɾ��ȫ����������
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo ��ȷ���Ƿ�ɾ��ȫ����������?
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

for /f "delims=" %%i in ('dir /b /a-d /s "*.bat"') do (
if not %0=="%%i" (
if not %cd%\==%%~dpi (
start /D %%~dpi deleteAll.bat Y
)
)
)

exit