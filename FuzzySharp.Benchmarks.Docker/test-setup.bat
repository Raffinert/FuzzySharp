@echo off
setlocal enabledelayedexpansion

echo === FuzzySharp Docker Benchmark Setup Test ===
echo.

:: Check if PowerShell is available
powershell -Command "Get-Host" >nul 2>&1
if errorlevel 1 (
    echo ERROR: PowerShell is not available
    echo Please install PowerShell or run the test manually
    pause
    exit /b 1
)

:: Run the PowerShell test script
echo Running Docker setup test...
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0test-docker-setup.ps1"

echo.
echo Press any key to continue...
pause >nul