@Title ɾ��Sqlite3��
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo ��ȷ���Ƿ�ɾ��Sqlite3��?
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

cd ./../../../Plugins

del /f /q /s Mono.Data.Sqlite.dll
del /f /q /s Mono.Data.Sqlite.dll.meta
del /f /q /s /ah Mono.Data.Sqlite.dll.meta

del /f /q /s Android\libsqlite3.so
del /f /q /s Android\libsqlite3.so.meta
del /f /q /s /ah Android\libsqlite3.so.meta

del /f /q /s x86\sqlite3.dll
del /f /q /s x86\sqlite3.dll.meta
del /f /q /s /ah x86\sqlite3.dll.meta

del /f /q /s x86_64\sqlite3.dll
del /f /q /s x86_64\sqlite3.dll.meta
del /f /q /s /ah x86_64\sqlite3.dll.meta

;�����Ƴ���ǰĿ¼ del /f /q /s %currentDirectory%.meta
;�����Ƴ���ǰĿ¼ del /f /q /s /ah %currentDirectory%.meta
;�����Ƴ���ǰĿ¼ rmdir /q /s %currentDirectory%

exit