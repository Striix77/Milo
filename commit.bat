@echo off
:: Combine all parameters into a single commit message
setlocal enabledelayedexpansion
set commitMessage=

:loop
if "%1"=="" goto done
set commitMessage=!commitMessage! %1
shift
goto loop

:done
:: Trim leading space
set commitMessage=!commitMessage:~1!

:: Check if a commit message is provided
if "!commitMessage!"=="" (
    echo Commit message is required.
    exit /b
)

:: Add all changes
git add .

:: Commit with the provided message
git commit -m "%*"

:: Push changes to the remote repository
git push origin main

:: Done
echo Commit successful!
pause
