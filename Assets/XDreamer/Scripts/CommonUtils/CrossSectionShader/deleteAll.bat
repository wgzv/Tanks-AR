@Title ɾ��Cross Section Shader��
@echo off

set one=%1

:condition

if "%one%"=="" (
	echo ��ȷ���Ƿ�ɾ��Cross Section Shader��?
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

ren "Cross Section Shader" CrossSectionShader
ren "Cross Section Shader.meta" CrossSectionShader.meta

rmdir /q /s %cd%\CrossSectionShader
del /f /q /s CrossSectionShader.meta
del /f /q /s /ah CrossSectionShader.meta

exit