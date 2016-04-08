var app;
(function (app) {
    var vacation;
    (function (vacation) {
        var Routes = (function () {
            function Routes() {
            }
            Routes.init = function ($stateProvider, $urlRouterProvider) {
                $stateProvider
                    .state("vacation.report", {
                    url: "/report",
                    templateUrl: "/App/Vacation/ReportVacation/ReportVacationView.html",
                    controller: "ReportVacationController",
                    controllerAs: "rvc",
                    resolve: {
                        adminEditCurrentUser: function () { return 0; },
                        ReportId: function () { return -1; },
                        CurrentUser: [
                            "$rootScope", "Person", function ($rootScope, Person) {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser().$promise.then(function (res) {
                                        $rootScope.CurrentUser = res;
                                    });
                                }
                                else {
                                    return $rootScope.CurrentUser;
                                }
                            }
                        ],
                        $modalInstance: function () { return -1; }
                    }
                })
                    .state("vacation.approvereports", {
                    url: "/approve-reports",
                    templateUrl: "/App/Vacation/ApproveReports/ApproveReportsView.html",
                    controller: "ApproveReportsController",
                    controllerAs: "arc",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", function ($rootScope, Person) {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser().$promise.then(function (res) {
                                        $rootScope.CurrentUser = res;
                                    });
                                }
                                else {
                                    return $rootScope.CurrentUser;
                                }
                            }
                        ],
                    }
                });
            };
            Routes.$inject = [
                "$stateProvider",
                "$urlRouterProvider"
            ];
            return Routes;
        })();
        vacation.Routes = Routes;
        angular.module("app.vacation").config(Routes.init);
    })(vacation = app.vacation || (app.vacation = {}));
})(app || (app = {}));
