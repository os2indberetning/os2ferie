angular.module("app", ["kendo.directives", "ui.router", "ui.bootstrap", "ui.bootstrap.tooltip", "ngResource", "angularMoment", "cgBusy", "app.drive", "app.vacation"])
    .config([
        'cfpLoadingBarProvider', function(cfpLoadingBarProvider) {
            cfpLoadingBarProvider.includeSpinner = false;
     
        }
    ]).run(['Person', "$rootScope", "HelpText", function(Person, $rootScope, HelpText) {

        if ($rootScope.CurrentUser == undefined) {
           $rootScope.CurrentUser = Person.GetCurrentUser().$promise.then(function (res) {
               $rootScope.CurrentUser = res;
               $rootScope.showAdministration = res.IsAdmin;
               $rootScope.showApproveReports = res.IsLeader || res.IsSubstitute;
               $rootScope.UserName = res.FullName;
               $rootScope.loaded = true;
           });
       }

        if($rootScope.HelpTexts == undefined) {
           HelpText.getAll().$promise.then(function(res) {
               $rootScope.helpLink = res.InformationHelpLink;
               $rootScope.HelpTexts = res;
           });
       }
    }]);

angular.module('app').value('cgBusyDefaults',{
  message:'Vent venligst..',
  backdrop: true,
  templateUrl: 'template/loading-template.html',
  delay: 100,
  minDuration: 700
});

angular.module('app').constant('angularMomentConfig', {
    preprocess: 'utc',
    timezone: 'Europe/Copenhagen'
});