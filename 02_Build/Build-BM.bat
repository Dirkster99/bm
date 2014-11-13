tools\nant\nant.exe init import build copy pack nuget -buildfile:bm.build -logfile:compile.txt 
if not ERRORLEVEL 0 pause
notepad compile.txt