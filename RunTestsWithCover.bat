@echo off
Call BuildVars.bat
.nuget\nuget install OpenCover -version %OPENCOVERVERSION% -o packages
packages\OpenCover.%OPENCOVERVERSION%\OpenCover.Console.exe -register:user -target:Build\Nunit.%NUNITVERSION%\bin\nunit-console-x86.exe -targetargs:"/noshadow /nodots Tests\bin\Debug\MixRadio.Tests.dll" -filter:"+[MixRadio.Tests]* -[MixRadio.Tests]MixRadio.Tests* -[MixRadio.Tests]Ionic*" -output:TestCoverage.xml

.nuget\nuget install ReportGenerator -version %REPORTGENERATORVERSION% -o packages
packages\ReportGenerator.%REPORTGENERATORVERSION%\ReportGenerator.exe -reports:TestCoverage.xml -sourcedirs:Tests -reporttypes:HTML -targetdir:TestCoverageReport
Start TestCoverageReport\index.htm
