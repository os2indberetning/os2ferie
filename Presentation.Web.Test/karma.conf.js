/// <reference path="C:\Users\JOJ\Documents\o2indberetning\Presentation.Web\App/tests/controllers/AcceptWithAccount.js" />
// Karma configuration
// Generated on Tue Feb 24 2015 09:24:24 GMT+0100 (Romance Standard Time)

module.exports = function(config) {
  config.set({

    // base path that will be used to resolve all patterns (eg. files, exclude)
    basePath: '',


    // frameworks to use
    // available frameworks: https://npmjs.org/browse/keyword/karma-adapter
    frameworks: ['jasmine'],


    // list of files / patterns to load in the browser
    files: [
        'https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js',
	    '../Presentation.Web/Scripts/angular.js',
        '../Presentation.Web/Scripts/angular-*.js',   // for angular.mock.module and inject.
        '../Presentation.Web/Scripts/angular-ui/*.js',
        'http://cdn.kendostatic.com/2014.3.1119/js/kendo.all.min.js',
        '../Presentation.Web/Scripts/moment.js',
        'AppTests/controllers/**/*.js',
        '../Presentation.Web/App/application.js',
        '../Presentation.Web/App/**/*.js'
        
    ],


    // list of files to exclude
    exclude: [
        '../Presentation.Web/App/AWS/*.js',
        '../Presentation.Web/**/angular-scenario.js',
        '../Presentation.Web/Scripts/kendo/2014.3.1119/angular*.js',
        '../Presentation.Web/Scripts/kendo/2014.3.1119/kendo.angular*'
    ],


    // preprocess matching files before serving them to the browser
    // available preprocessors: https://npmjs.org/browse/keyword/karma-preprocessor
    preprocessors: {
        '../Presentation.Web/App/**/*.js': 'coverage'
    },


    // test results reporter to use
    // possible values: 'dots', 'progress'
    // available reporters: https://npmjs.org/browse/keyword/karma-reporter
    reporters: ['progress', 'coverage'],


    // web server port
    port: 9876,


    // enable / disable colors in the output (reporters and logs)
    colors: true,


    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,


    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,


    // start these browsers
    // available browser launchers: https://npmjs.org/browse/keyword/karma-launcher
    browsers: ['Chrome', 'PhantomJS', 'Firefox', 'IE'],


    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits
    singleRun: false
  });
};
