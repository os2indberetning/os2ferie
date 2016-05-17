module app.vacation {

    export class Routes {
        static $inject = [
            "$stateProvider",
            "$urlRouterProvider"
        ];

        static init($stateProvider: ng.ui.IStateProvider, $urlRouterProvider: ng.ui.IUrlRouterProvider) {
            $stateProvider
                .state("vacation.report", {
                    url: "/report",
                    templateUrl: "/App/Vacation/ReportVacation/ReportVacationView.html",
                    controller: "ReportVacationController",
                    controllerAs: "rvc",
                    resolve: {
                        vacationReportId () { return 0; },
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser().$promise.then(res => {
                                        $rootScope.CurrentUser = res;
                                    });
                                } else {
                                    return $rootScope.CurrentUser;
                                }

                            }
                        ],
                        $modalInstance() { return -1; }
                    }
                })
                .state("vacation.approvereports", {
                    url: "/approve-reports",
                    templateUrl: "/App/Vacation/ApproveReports/ApproveReportsView.html",
                    controller: "ApproveReportsController",
                    controllerAs: "arc",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser().$promise.then(res => {
                                        $rootScope.CurrentUser = res;
                                    });
                                } else {
                                    return $rootScope.CurrentUser;
                                }

                            }
                        ]
                    }
                })
                .state("vacation.approvereportssettings", {
                    url: "/approve-reports-settings",
                    templateUrl: "/App/Core/Views/ApproveReportsSettingsView.html",
                    controller: "Vacation.ApproveVacationReportsSettingsController",
                    controllerAs: "arsCtrl",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser().$promise.then(res => {
                                        $rootScope.CurrentUser = res;
                                    });
                                } else {
                                    return $rootScope.CurrentUser;
                                }

                            }
                        ]
                    }
                })
                .state("vacation.admin", {
                    url: "/admin",
                    templateUrl: "/App/Vacation/Admin/AdminView.html",
                    controller: "Vacation.AdminMenuController",
                    controllerAs: "ctrl",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser().$promise.then(res => {
                                        $rootScope.CurrentUser = res;
                                    });
                                } else {
                                    return $rootScope.CurrentUser;
                                }

                            }
                        ]
                    }
                })
                .state("vacation.myreports", {
                    url: "/myreports",
                    templateUrl: "/App/Vacation/MyVacationReports/MyVacationReportsView.html",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser().$promise.then(res => {
                                        $rootScope.CurrentUser = res;
                                    });
                                } else {
                                    return $rootScope.CurrentUser;
                                }

                            }
                        ]
                    }
                });
        }
    }

    angular.module("app.vacation").config(Routes.init);
}