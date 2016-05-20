module app {
    "use strict";

    var cgBusyDefaultsConfig = {
        message: 'Vent venligst..',
        backdrop: true,
        templateUrl: 'template/loading-template.html',
        delay: 100,
        minDuration: 700
    }

    var momentConfig = {
        preprocess: (data) => {
            return moment.unix(data);
        },
        timezone: 'Europe/Copenhagen'
    }

    angular.module("app", [
        "kendo.directives",
        "ui.router",
        "ui.bootstrap",
        "ui.bootstrap.tooltip",
        "ngResource",
        "angularMoment",
        "cgBusy",
        "app.drive",
        "app.vacation"])
        .config([
            'cfpLoadingBarProvider', (cfpLoadingBarProvider) => {
                cfpLoadingBarProvider.includeSpinner = false;
            }
        ])
        .run(([
            'Person', '$rootScope', "HelpText", (Person, $rootScope, HelpText) => {
                if ($rootScope.CurrentUser == undefined) {
                    $rootScope.CurrentUser = Person.GetCurrentUser().$promise.then(res => {
                        $rootScope.CurrentUser = res;
                        $rootScope.showAdministration = res.IsAdmin;
                        $rootScope.showApproveReports = res.IsLeader || res.IsSubstitute;
                        $rootScope.UserName = res.FullName;
                        $rootScope.loaded = true;
                    });
                }

                if ($rootScope.HelpTexts == undefined) {
                    HelpText.getAll().$promise.then(res => {
                        $rootScope.helpLink = res.InformationHelpLink;
                        $rootScope.HelpTexts = res;
                    });
                }
            }
        ]))
        .constant('moment', moment)
        .constant('angularMomentConfig', momentConfig)
        .value("cgBusyDefaults", cgBusyDefaultsConfig);
}