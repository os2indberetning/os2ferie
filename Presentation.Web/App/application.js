var application = angular.module("application", ["kendo.directives", "ui.router", "ui.bootstrap", "ui.bootstrap.tooltip", "ngResource", "template/modal/window.html", "template/modal/window.html", "template/modal/backdrop.html", "template/tabs/tab.html", "template/tabs/tabset.html", "angularMoment", "template/popover/popover.html", "kendo-ie-fix"]);

angular.module("application").config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/");

    $stateProvider
        .state("Default", {
            url: "/",
            templateUrl: "/App/Driving/DrivingView.html"
        })
        .state("driving", {
            url: "/driving",
            templateUrl: "/App/Driving/DrivingView.html"
        })
        .state("myreports", {
            url: "/myreports",
            templateUrl: "/App/MyReports/MyReportsView.html"
        })
        .state("approvereports", {
            url: "/approvereports",
            templateUrl: "/App/ApproveReports/ApproveReportsView.html"
        })
        .state("settings", {
            url: "/settings",
            templateUrl: "/App/Settings/SettingsView.html"
        })
        .state("admin", {
            url: "/admin",
            templateUrl: "/App/Admin/AdminView.html"
        });
}]);

application.constant('angularMomentConfig', {
    preprocess: 'utc',
    timezone: 'Europe/Copenhagen'
});