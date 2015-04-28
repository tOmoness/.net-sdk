@echo off
Call BuildVars.bat
Build\Nunit.%NUNITVERSION%\bin\nunit-console.exe /noshadow Tests\bin\Debug\MixRadio.Tests.dll
