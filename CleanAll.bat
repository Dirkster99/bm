@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO BmLib
ECHO.
ECHO ShellBrowser\Client
ECHO ShellBrowser\WSF
ECHO ShellBrowser\UnitTestWSF
ECHO ShellBrowser\SSCCoreLib
ECHO ShellBrowser\PerformanceTestClient
ECHO WpfPerformance
ECHO.
ECHO Icons
ECHO.
ECHO Components\ServiceLocator
ECHO Components\Settings\Settings
ECHO Components\Settings\SettingsModel
ECHO.
ECHO Demos\GenericDemo
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
RMDIR /S /Q TestResults

ECHO.
ECHO Deleting BIN and OBJ Folders in BmLib folder
ECHO.
RMDIR /S /Q BmLib\bin
RMDIR /S /Q BmLib\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in ShellBrowserLib folder
ECHO.
RMDIR /S /Q ShellBrowser\WSF\bin
RMDIR /S /Q ShellBrowser\WSF\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in Client folder
ECHO.
RMDIR /S /Q ShellBrowser\Client\bin
RMDIR /S /Q ShellBrowser\Client\obj

ECHO.
ECHO Deleting BIN and OBJ Folders in UnitTests folder
ECHO.
RMDIR /S /Q ShellBrowser\UnitTestWSF\bin
RMDIR /S /Q ShellBrowser\UnitTestWSF\obj

ECHO Deleting BIN and OBJ Folders in ShellBrowser\SSCCoreLib folder
ECHO.
RMDIR /S /Q ShellBrowser\SSCCoreLib\bin
RMDIR /S /Q ShellBrowser\SSCCoreLib\obj

ECHO Deleting BIN and OBJ Folders in ShellBrowser\PerformanceTestClient folder
ECHO.
RMDIR /S /Q ShellBrowser\PerformanceTestClient\bin
RMDIR /S /Q ShellBrowser\PerformanceTestClient\obj

ECHO Deleting BIN and OBJ Folders in WpfPerformance folder
ECHO.
RMDIR /S /Q WpfPerformance\bin
RMDIR /S /Q WpfPerformance\obj

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

ECHO Deleting BIN and OBJ Folders in GenericDemo folder
ECHO.
RMDIR /S /Q Demos\GenericDemo\bin
RMDIR /S /Q Demos\GenericDemo\obj

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