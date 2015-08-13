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
                    if ($rootScope.CurrentUser == undefined) {
                        return Person.GetCurrentUser().$promise.then(function (res) {
                            $rootScope.CurrentUser = res;
                        });
                    } else {
                        return $rootScope.CurrentUser;
                    }
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
                    if ($rootScope.CurrentUser == undefined) {
                        return Person.GetCurrentUser().$promise.then(function (res) {
                            $rootScope.CurrentUser = res;
                        });
                    } else {
                        return $rootScope.CurrentUser;
                    }

                }],
                $modalInstance: function () { return -1; }
            }
        })
        .state("myreports", {
            url: "/myreports",
            templateUrl: "/App/MyReports/MyReportsView.html",
            resolve: {
                CurrentUser: ["$rootScope", "Person", function ($rootScope, Person) {
                    if ($rootScope.CurrentUser == undefined) {
                        return Person.GetCurrentUser().$promise.then(function (res) {
                            $rootScope.CurrentUser = res;
                        });
                    } else {
                        return $rootScope.CurrentUser;
                    }
                }]
            }
        })
        .state("approvereports", {
            url: "/approvereports",
            templateUrl: "/App/ApproveReports/ApproveReportsView.html",
            resolve: {
                CurrentUser: ["Person", "$location", "$rootScope", function (Person, $location, $rootScope) {
                    if ($rootScope.CurrentUser == undefined) {
                        return Person.GetCurrentUser().$promise.then(function (data) {
                            $rootScope.CurrentUser = data;
                            if (!data.IsLeader && !data.IsSubstitute) {
                                $location.path("driving");
                            }
                        });
                    } else {
                        if (!$rootScope.CurrentUser.IsLeader && !$rootScope.CurrentUser.IsSubstitute) {
                            $location.path("driving");
                        }
                        return $rootScope.CurrentUser;
                    }
                }],
                OrgUnits: ["$rootScope", "OrgUnit", function ($rootScope, OrgUnit) {
                    if ($rootScope.OrgUnits == undefined) {
                        return OrgUnit.get({ query: "$select=Id, LongDescription, HasAccessToFourKmRule" }).$promise.then(function (res) {
                            $rootScope.OrgUnits = res.value;
                        });
                    } else {
                        return $rootScope.OrgUnits;
                    }
                }],
                People: ["$rootScope", "Person", function ($rootScope, Person) {
                    if ($rootScope.People == undefined) {
                        return Person.getAll({ query: "$select=Id,FullName,IsActive" }).$promise.then(function (res) {
                            $rootScope.People = res.value;
                        });
                    } else {
                        return $rootScope.People;
                    }
                }]

            }
        })
        .state("settings", {
            url: "/settings",
            templateUrl: "/App/Settings/SettingsView.html",
            controller: "SettingController",
            resolve: {
                CurrentUser: ["$rootScope", "Person", function ($rootScope, Person) {
                    if ($rootScope.CurrentUser == undefined) {
                        return Person.GetCurrentUser().$promise.then(function (res) {
                            $rootScope.CurrentUser = res;
                        });
                    } else {
                        return $rootScope.CurrentUser;
                    }
                }]
            }
        })
        .state("admin", {
            url: "/admin",
            templateUrl: "/App/Admin/AdminView.html",
            controller: "AdminMenuController",
            resolve: {
                CurrentUser: ["Person", "$location", "$rootScope", function (Person, $location, $rootScope) {
                    if ($rootScope.CurrentUser == undefined) {
                        return Person.GetCurrentUser().$promise.then(function (data) {
                            $rootScope.CurrentUser = data;
                            if (!data.IsAdmin) {
                                $location.path("driving");
                            }
                        });
                    } else {
                        if (!$rootScope.CurrentUser.IsAdmin) {
                            $location.path("driving");
                        }
                        return $rootScope.CurrentUser;
                    }
                }],
                OrgUnits: ["$rootScope", "OrgUnit", function ($rootScope, OrgUnit) {
                    if ($rootScope.OrgUnits == undefined) {
                        return OrgUnit.get({ query: "$select=Id, LongDescription, HasAccessToFourKmRule" }).$promise.then(function (res) {
                            $rootScope.OrgUnits = res.value;
                        });
                    } else {
                        return $rootScope.OrgUnits;
                    }
                }],
                People: ["$rootScope", "Person", function ($rootScope, Person) {
                    if ($rootScope.People == undefined) {
                        return Person.getAll({ query: "$select=Id,FullName,IsActive" }).$promise.then(function (res) {
                            $rootScope.People = res.value;
                        });
                    } else {
                        return $rootScope.People;
                    }
                }]
            }
        });
}]);

application.constant('angularMomentConfig', {
    preprocess: 'utc',
    timezone: 'Europe/Copenhagen'
});