angular.module('app.drive', ["app.common", "kendo.directives", "ui.router", "ui.bootstrap", "ui.bootstrap.tooltip", "ngResource", "template/modal/window.html", "template/modal/window.html", "template/modal/backdrop.html", "template/tabs/tab.html", "template/tabs/tabset.html", "angularMoment", "template/popover/popover.html", "kendo-ie-fix", 'angular-loading-bar'])
    .run(['$rootScope','HelpText', function($rootScope, HelpText) {
       if($rootScope.HelpTexts == undefined) {
           HelpText.getAll().$promise.then(function(res) {
               $rootScope.helpLink = res.InformationHelpLink;
               $rootScope.HelpTexts = res;
           });
       }
    }]);