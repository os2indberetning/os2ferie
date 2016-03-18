angular.module("app.drive").config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/");

    $stateProvider
        .state("drive.default", {
            url: "/",
            templateUrl: "/App/Drive/Driving/DrivingView.html",
            controller: "DrivingController",
            resolve: {
                adminEditCurrentUser : function() {return 0;},
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
        .state("drive.driving", {
            url: "/driving",
            templateUrl: "/App/Drive/Driving/DrivingView.html",
            controller: "DrivingController",
            resolve: {
                adminEditCurrentUser : function() {return 0;},
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
        .state("drive.myreports", {
            url: "/myreports",
            templateUrl: "/App/Drive/MyReports/MyReportsView.html",
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
        .state("drive.approvereports", {
            url: "/approvereports",
            templateUrl: "/App/Drive/ApproveReports/ApproveReportsView.html",
            resolve: {
                CurrentUser: ["Person", "$location", "$rootScope", function (Person, $location, $rootScope) {
                    if ($rootScope.CurrentUser == undefined || ($rootScope.CurrentUser.$$state != undefined && $rootScope.CurrentUser.$$state.status == 0)) {
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
            }
        })
        .state("drive.settings", {
            url: "/settings",
            templateUrl: "/App/Drive/Settings/SettingsView.html",
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
        .state("drive.admin", {
            url: "/admin",
            templateUrl: "/App/Drive/Admin/AdminView.html",
            controller: "AdminMenuController",
            resolve: {
                CurrentUser: ["Person", "$location", "$rootScope", function (Person, $location, $rootScope) {
                    if ($rootScope.CurrentUser == undefined || ($rootScope.CurrentUser.$$state != undefined && $rootScope.CurrentUser.$$state.status == 0)) {

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
            }
        });
}]);