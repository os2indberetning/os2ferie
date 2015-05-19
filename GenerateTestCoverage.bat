REM Run ApplicationServices.Test and generate a report for the coverage

packages\OpenCover.4.5.3522\OpenCover.Console.exe -target:RunApplicationServicesTests.bat -register:user -filter:+[Core.ApplicationServices]* 
packages\ReportGenerator.2.1.1.0\reportgenerator.exe -reports:results.xml -targetdir:coverage/ApplicationServices


REM Run Presentation.Web.Test and generate a report for the coverage

packages\OpenCover.4.5.3522\OpenCover.Console.exe -target:RunPresentationWebTests.bat -register:user -filter:+[OS2Indberetning]* 
packages\ReportGenerator.2.1.1.0\reportgenerator.exe -reports:results.xml -targetdir:coverage/PresentationWeb


REM Run ConsoleApplications tests and generate a report containing the coverage

packages\OpenCover.4.5.3522\OpenCover.Console.exe -target:RunConsoleApplicationsTests.bat -register:user -filter:+* 
packages\ReportGenerator.2.1.1.0\reportgenerator.exe -reports:results.xml -targetdir:coverage/ConsoleApplications


REM Run DBUpdater tests and generate a report containing the coverage

packages\OpenCover.4.5.3522\OpenCover.Console.exe -target:RunDBUpdaterTests.bat -register:user -filter:+[*]DBUpdater* -filter:-[*]DBUpdater.Program -filter:-[*]DBUpdater.SyddjursDataProvider -filter:-[*]Core.DomainModel.*
packages\ReportGenerator.2.1.1.0\reportgenerator.exe -reports:results.xml -targetdir:coverage/DBUpdater



REM Run all tests and generate one report containing the coverage

packages\OpenCover.4.5.3522\OpenCover.Console.exe -target:RunAllTests.bat -register:user -filter:+* 
packages\ReportGenerator.2.1.1.0\reportgenerator.exe -reports:results.xml -targetdir:coverage/AllTests

