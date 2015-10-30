@echo off
pushd ..\BclEx-Abstract
echo Building BclEx-Abstract:
::PowerShell -Command ".\psake.ps1"
popd

::
echo BclEx-Abstract
mkdir packages\BclEx-Abstract.1.0.0
pushd packages\BclEx-Abstract.1.0.0
set SRC=..\..\..\BclEx-Abstract\Release
xcopy %SRC%\BclEx-Abstract.1.0.0.nupkg . /Y/Q
..\..\tools\7za.exe x -y BclEx-Abstract.1.0.0.nupkg -ir!lib
popd
::pause