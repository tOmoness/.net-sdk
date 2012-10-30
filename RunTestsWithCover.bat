@echo off
Call BuildVars.bat
.nuget\nuget install OpenCover -version %OPENCOVERVERSION% -o packages
packages\OpenCover.%OPENCOVERVERSION%\OpenCover.Console.exe -register:user -target:Build\Nunit.%NUNITVERSION%\bin\nunit-console-x86.exe -targetargs:"/noshadow /nodots NokiaMusicApiTests\bin\Debug\Nokia.Music.Tests.dll" -filter:"+[Nokia.Music.Tests]* -[Nokia.Music.Tests]Nokia.Music.Phone.Tests*" -output:TestCoverage.xml

.nuget\nuget install ReportGenerator -version %REPORTGENERATORVERSION% -o packages
packages\ReportGenerator.%REPORTGENERATORVERSION%\ReportGenerator.exe -reports:TestCoverage.xml -sourcedirs:NokiaMusicApiTests -reporttypes:HTML -targetdir:TestCoverageReport
Start TestCoverageReport\index.htm
