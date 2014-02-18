@echo off

REM Run the CI script
.\ContinuousIntegration\Tools\psake\psake.cmd .\ContinuousIntegration\Run-Ci.ps1 -framework 4.5.1 %*