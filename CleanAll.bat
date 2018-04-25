@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO BreadcrumbLib
ECHO.
ECHO DirectoryInfoExLib
ECHO Icons
ECHO.
ECHO Components\ServiceLocator
ECHO Components\Settings\Settings
ECHO Components\Settings\SettingsModel
ECHO.
ECHO Demos\Breadcrumb
ECHO Demos\BreadcrumbTestLib
ECHO Demos\ThemedDemo
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

RMDIR /S /Q .vs

ECHO.
ECHO Deleting BIN and OBJ Folders in BreadcrumbLib folder
ECHO.
RMDIR /S /Q BreadcrumbLib\bin
RMDIR /S /Q BreadcrumbLib\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in DirectoryInfoExLib folder
ECHO.
RMDIR /S /Q Components\DirectoryInfoExLib\bin
RMDIR /S /Q Components\DirectoryInfoExLib\obj

ECHO Deleting BIN and OBJ Folders in Icons folder
ECHO.
RMDIR /S /Q Components\Icons\bin
RMDIR /S /Q Components\Icons\obj

ECHO.

ECHO Deleting BIN and OBJ Folders in ServiceLocator folder
ECHO.
RMDIR /S /Q Components\ServiceLocator\bin
RMDIR /S /Q Components\ServiceLocator\obj

ECHO Deleting BIN and OBJ Folders in Settings\Settings folder
ECHO.
RMDIR /S /Q Components\Settings\Settings\bin
RMDIR /S /Q Components\Settings\Settings\obj

ECHO Deleting BIN and OBJ Folders in Settings\SettingsModel folder
ECHO.
RMDIR /S /Q Components\Settings\SettingsModel\bin
RMDIR /S /Q Components\Settings\SettingsModel\obj

ECHO.


ECHO Deleting BIN and OBJ Folders in Breadcrumb folder
ECHO.
RMDIR /S /Q Demos\Breadcrumb\bin
RMDIR /S /Q Demos\Breadcrumb\obj

ECHO Deleting BIN and OBJ Folders in BreadcrumbTestLib folder
ECHO.
RMDIR /S /Q Demos\BreadcrumbTestLib\bin
RMDIR /S /Q Demos\BreadcrumbTestLib\obj

ECHO Deleting BIN and OBJ Folders in ThemedDemo folder
ECHO.
RMDIR /S /Q Demos\ThemedDemo\bin
RMDIR /S /Q Demos\ThemedDemo\obj

PAUSE

:EndOfBatch