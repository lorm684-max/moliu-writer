@echo off
setlocal

cd /d "%~dp0.."

set "GH=gh"
where gh >nul 2>nul
if errorlevel 1 (
  if exist "C:\Program Files\GitHub CLI\gh.exe" set "GH=C:\Program Files\GitHub CLI\gh.exe"
)

"%GH%" --version >nul 2>nul
if errorlevel 1 (
  echo GitHub CLI was not found. Please install GitHub CLI first.
  pause
  exit /b 1
)

"%GH%" auth status >nul 2>nul
if errorlevel 1 (
  echo Please log in to GitHub in the browser window.
  "%GH%" auth login --hostname github.com --web --git-protocol https
  if errorlevel 1 (
    pause
    exit /b 1
  )
)

git status >nul 2>nul
if errorlevel 1 (
  echo This folder is not a Git repository.
  pause
  exit /b 1
)

git remote get-url origin >nul 2>nul
if errorlevel 1 (
  "%GH%" repo create moliu-writer --private --source . --remote origin --push
) else (
  git push -u origin main
)

echo.
echo Done.
pause

