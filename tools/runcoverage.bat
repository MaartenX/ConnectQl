@echo off

setlocal enabledelayedexpansion

SET cwd=%~dp0
SET err=0

nuget install OpenCover -ExcludeVersion > NUL
nuget install Codecov -ExcludeVersion > NUL

OpenCover\tools\OpenCover.Console.exe -oldStyle -register:user "-target:%cwd%\runtests.bat" "-filter:+[ConnectQl*]* -[*Tests*]* -[*]JetBrains.*" "-output:artifacts\ConnectQl.Coverage.xml" -returntargetcode
s
IF ERRORLEVEL 1 ( SET err=1 )

Codecov\tools\codecov.exe -f "artifacts\ConnectQl.Coverage.xml"

endlocal & cmd /c exit /b %err%