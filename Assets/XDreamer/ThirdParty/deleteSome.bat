@Title ɾ��ĳЩ��
@echo off

set lib=EasyAR,GoogleVR,HoloLens,zSpace,HTCVive,SteamVR

set one=%1

:condition

if "%one%"=="" (
	echo ��ȷ���Ƿ�ɾ��%lib%��?
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
			for %%l in (%lib%) do (	
				if "%cd%\%%l\"=="%%~dpi" (
					start /D %%~dpi deleteAll.bat Y
				)
			)
		)
	)
)

exit