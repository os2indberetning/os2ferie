/**
    Information about casperjs:

    First off you need to have python, phamtomjs and casperjs installed and added to your
    path environmental variable.
    Notice that the current (as per 24 february 2015) npm installation of casper has a 
    bug that prevents it from running, so it should be installed from git instead.

    Tests with casperjs is run by executing the run-casper-tests.bat script.
    The script will run all .js files that is present in FrontendCasperTests and its
    sub folders. All js files in these folders must be valid casper test files or else
    it will halt the execution.
*/




var domain = "http://localhost:1983/";