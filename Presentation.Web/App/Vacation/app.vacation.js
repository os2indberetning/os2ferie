angular.module('app.vacation', ["app.common", "kendo.directives", "ui.router", "ui.bootstrap", "ui.bootstrap.tooltip", "ngResource", "template/modal/window.html", "template/modal/window.html", "template/modal/backdrop.html", "template/tabs/tab.html", "template/tabs/tabset.html", "angularMoment", "template/popover/popover.html", "kendo-ie-fix", 'angular-loading-bar'])
    .run(['$rootScope','HelpText', 'Person', function($rootScope, HelpText, Person) {
       if ($rootScope.CurrentUser == undefined) {
            $rootScope.CurrentUser = Person.GetCurrentUser().$promise.then(function(res) {
                $rootScope.CurrentUser = res;
            });
        }

        if($rootScope.HelpTexts == undefined) {
           HelpText.getAll().$promise.then(function(res) {
               $rootScope.helpLink = res.InformationHelpLink;
               $rootScope.HelpTexts = res;
           });
       }

    }]);