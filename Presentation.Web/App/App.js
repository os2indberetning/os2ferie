/// <reference path="../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../scripts/typings/angular-ui/angular-ui-router.d.ts" />
var Application;
(function (Application) {
    "use strict";
    var StateProviderConfig = (function () {
        function StateProviderConfig($stateProvider) {
            this.$stateProvider = $stateProvider;
            this.init();
        }
        StateProviderConfig.prototype.init = function () {
            this.$stateProvider.state("app", {
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
        };
        return StateProviderConfig;
    })();
    var AngularMomentConfig = (function () {
        function AngularMomentConfig() {
            this.preprocess = 'utc';
            this.timezone = 'Europe/Copenhagen';
        }
        return AngularMomentConfig;
    })();
    var AngularApp = (function () {
        function AngularApp() {
        }
        AngularApp.Module = angular.module("app", ["kendo.directives", "ui.router", "ui.bootstrap", "template/tabs/tab.html", "template/tabs/tabset.html", "angularMoment"]).config(["$stateProvider", function ($stateProvider) {
            return new StateProviderConfig($stateProvider);
        }]).constant('angularMomentConfig', new AngularMomentConfig());
        return AngularApp;
    })();
    Application.AngularApp = AngularApp;
})(Application || (Application = {}));
//# sourceMappingURL=App.js.map