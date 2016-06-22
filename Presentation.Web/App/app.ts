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
        "app.core",
        "app.drive",
        "app.vacation"])
        .config([
            'cfpLoadingBarProvider', (cfpLoadingBarProvider) => {
                cfpLoadingBarProvider.includeSpinner = false;
            }
        ])
        .run(([
            "Person", "$rootScope", "HelpText", '$state', '$stateParams',
            'Authorization', 'Principal', (Person, $rootScope, HelpText, $state, $stateParams,
                Authorization, Principal) => {

                $rootScope.hasAccess = true;

                if ($rootScope.HelpTexts == undefined) {
                    HelpText.getAll().$promise.then(res => {
                        $rootScope.helpLink = res.InformationHelpLink;
                        $rootScope.HelpTexts = res;
                    });
                }

                $rootScope.$on('$stateChangeStart',
                    (event, toState, toStateParams) => {
                        // track the state the user wants to go to;
                        // authorization service needs this
                        $rootScope.toState = toState;
                        $rootScope.toStateParams = toStateParams;

                        // if the principal is resolved, do an
                        // authorization check immediately. otherwise,
                        // it'll be done when the state is resolved.
                        if (Principal.isIdentityResolved())
                            Authorization.authorize();
                    });
            }
        ]))
        .constant('moment', moment)
        .constant('angularMomentConfig', momentConfig)
        .value("cgBusyDefaults", cgBusyDefaultsConfig);
}