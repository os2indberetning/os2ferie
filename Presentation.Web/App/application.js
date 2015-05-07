var application = angular.module("application", ["kendo.directives", "ui.router", "ui.bootstrap", "ui.bootstrap.tooltip", "ngResource", "template/modal/window.html", "template/modal/window.html", "template/modal/backdrop.html", "template/tabs/tab.html", "template/tabs/tabset.html", "angularMoment", "template/popover/popover.html", "kendo-ie-fix", 'angular-loading-bar'])
    .config([
        'cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
            cfpLoadingBarProvider.includeSpinner = false;
        }
    ]);

angular.module("application").config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/");

    $stateProvider
        .state("Default", {
            url: "/",
            templateUrl: "/App/Driving/DrivingView.html",
            controller: "DrivingController",
            resolve: {
                ReportId: function () { return -1; },
                CurrentUser: ["$rootScope", "Person", function ($rootScope, Person) {
                    return Person.GetCurrentUser().$promise.then(function (res) {
                        $rootScope.CurrentUser = res;
                    });
                }],
                $modalInstance: function () { return -1; }

            }
        })
        .state("driving", {
            url: "/driving",
            templateUrl: "/App/Driving/DrivingView.html",
            controller: "DrivingController",
            resolve: {
                ReportId: function () { return -1; },
                CurrentUser: ["$rootScope", "Person", function ($rootScope, Person) {
                    return Person.GetCurrentUser().$promise.then(function (res) {
                        $rootScope.CurrentUser = res;
                    });
                }],
                $modalInstance: function () { return -1; }
            }
        })
        .state("myreports", {
            url: "/myreports",
            templateUrl: "/App/MyReports/MyReportsView.html",
            resolve: {
                CurrentUser: ["$rootScope", "Person", function ($rootScope, Person) {
                    return Person.GetCurrentUser().$promise.then(function (res) {
                        $rootScope.CurrentUser = res;
                    });
                }]
            }
        })
        .state("approvereports", {
            url: "/approvereports",
            templateUrl: "/App/ApproveReports/ApproveReportsView.html",
            resolve: {
                CurrentUser: ["Person", "$location", "$rootScope", function (Person, $location, $rootScope) {
                    return Person.GetCurrentUser().$promise.then(function (data) {
                        $rootScope.CurrentUser = data;
                        if (!data.IsLeader) {
                            $location.path("driving");
                        }
                    });
                }]
            }
        })
        .state("settings", {
            url: "/settings",
            templateUrl: "/App/Settings/SettingsView.html",
            controller: "SettingController",
            resolve: {
                CurrentUser: ["$rootScope", "Person", function ($rootScope, Person) {
                    return Person.GetCurrentUser().$promise.then(function (res) {
                        $rootScope.CurrentUser = res;
                    });
                }]
            }
        })
        .state("admin", {
            url: "/admin",
            templateUrl: "/App/Admin/AdminView.html",
            resolve: {
                CurrentUser: ["Person", "$location", "$rootScope", function (Person, $location, $rootScope) {
                    return Person.GetCurrentUser().$promise.then(function (data) {
                        $rootScope.CurrentUser = data;
                        if (!data.IsAdmin) {
                            $location.path("driving");
                        }
                    });
                }]
            }
        });
}]);

application.constant('angularMomentConfig', {
    preprocess: 'utc',
    timezone: 'Europe/Copenhagen'
});