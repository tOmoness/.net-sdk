@echo off
Call BuildVars.bat
Build\Nunit.%NUNITVERSION%\bin\nunit-console.exe /noshadow NokiaMusicApiTests\bin\Debug\Nokia.Music.Tests.dll
