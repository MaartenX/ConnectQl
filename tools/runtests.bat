@echo off

setlocal enabledelayedexpansion

SET cwd=%~dp0
SET src=%cwd:~0,-6%src
SET err=0

nuget install xunit.runner.console -ExcludeVersion > NUL

for /f "usebackq delims=|" %%f in (`dir /ad /b "%src%\*tests*"`) do (

    IF EXIST "%src%\%%f\bin\%configuration%\netcoreapp1.1\%%f.dll" (
        echo "c:\program files\dotnet\dotnet.exe" "xunit.runner.console\tools\netcoreapp2.0\xunit.console.dll" "%src%\%%f\bin\%configuration%\netcoreapp1.1\%%f.dll" -noshadow
        "c:\program files\dotnet\dotnet.exe" "xunit.runner.console\tools\netcoreapp2.0\xunit.console.dll" "%src%\%%f\bin\%configuration%\netcoreapp1.1\%%f.dll" -noshadow
        if ERRORLEVEL 1 (SET err=1)
    )

    IF EXIST "%src%\%%f\bin\%configuration%\net452\%%f.dll" (
        echo "xunit.runner.console\tools\net452\xunit.console.exe" "%src%\%%f\bin\%configuration%\net452\%%f.dll" -noshadow 
        "xunit.runner.console\tools\net452\xunit.console.exe" "%src%\%%f\bin\%configuration%\net452\%%f.dll" -noshadow 
        if ERRORLEVEL 1 (SET err=1)
    )
)

endlocal& cmd /c exit /b %err%