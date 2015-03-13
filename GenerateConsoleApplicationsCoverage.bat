

REM Run ConsoleApplications tests and generate a report containing the coverage

packages\OpenCover.4.5.3522\OpenCover.Console.exe -target:RunConsoleApplicationsTests.bat -register:user -filter:+[*]Mail.*
packages\ReportGenerator.2.1.1.0\reportgenerator.exe -reports:results.xml -targetdir:coverage/ConsoleApplications

