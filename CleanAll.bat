@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO BreadcrumbLib
ECHO Breadcrumb
ECHO ThemesManager
ECHO Icons
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

ECHO Deleting BIN and OBJ Folders in BreadcrumbLib folder
ECHO.
RMDIR /S /Q BreadcrumbLib\bin
RMDIR /S /Q BreadcrumbLib\obj

ECHO Deleting BIN and OBJ Folders in Breadcrumb folder
ECHO.
RMDIR /S /Q Breadcrumb\bin
RMDIR /S /Q Breadcrumb\obj


ECHO Deleting BIN and OBJ Folders in ThemesManager folder
ECHO.
RMDIR /S /Q ThemesManager\bin
RMDIR /S /Q ThemesManager\obj

ECHO Deleting BIN and OBJ Folders in Icons folder
ECHO.
RMDIR /S /Q Icons\bin
RMDIR /S /Q Icons\obj

PAUSE

:EndOfBatch