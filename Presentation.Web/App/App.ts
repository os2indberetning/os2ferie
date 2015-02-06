/// <reference path="../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../Scripts/typings/angular-ui/angular-ui-router.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

module Application {
    "use strict";

    class StateProviderConfig {
        constructor(private $stateProvider: ng.ui.IStateProvider)
        {
            this.init();            
        }
        private init(): void {
            this.$stateProvider.state("app", <ng.ui.IState>
                {
                    abstract: true,
                });
            this.$stateProvider.state("Default", {
                url: "",
                templateUrl: "/App/Frontpage/FrontpageView.html"
            });
            this.$stateProvider.state("frontpage", {
                url: "/frontpage",
                templateUrl: "/App/Frontpage/FrontpageView.html"
            });
            this.$stateProvider.state("driving", {
                url: "/driving",
                templateUrl: "/App/Driving/DrivingView.html"
            });
            this.$stateProvider.state("myreports", {
                url: "/myreports",
                templateUrl: "/App/MyReports/MyReportsView.html"
            });
            this.$stateProvider.state("approvereports", {
                url: "/approvereports",
                templateUrl: "/App/ApproveReports/ApproveReportsView.html"
            });
            this.$stateProvider.state("settings", {
                url: "/settings",
                templateUrl: "/App/Settings/SettingsView.html"
            });
            this.$stateProvider.state("admin", {
                url: "/admin",
                templateUrl: "/App/Admin/AdminView.html"
            });
        }
    }

    class AngularMomentConfig {
        preprocess = 'utc';
        timezone = 'Europe/Copenhagen';
    }
    
    export class AngularApp {
        public static Module: ng.IModule = angular.module("app", ["kendo.directives", "ui.router", "ui.bootstrap", "template/tabs/tab.html", "template/tabs/tabset.html", "angularMoment"])
            .config(
                ["$stateProvider",
                    ($stateProvider) => {
                        return new StateProviderConfig($stateProvider);
                    }
                ])
            .constant('angularMomentConfig', new AngularMomentConfig())
            .run((amMoment) => (amMoment.changeLocale('da')));
    }
}



