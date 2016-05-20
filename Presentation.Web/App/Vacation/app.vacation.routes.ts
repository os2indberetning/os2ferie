module app.vacation {

    export class Routes {
        static $inject = [
            "$stateProvider",
            "$urlRouterProvider"
        ];

        static init($stateProvider: ng.ui.IStateProvider, $urlRouterProvider: ng.ui.IUrlRouterProvider) {
            $stateProvider
                .state("vacation.report",
                {
                    url: "/report",
                    templateUrl: "/App/Vacation/ReportVacation/ReportVacationView.html",
                    controller: "CreateReportVacationController",
                    controllerAs: "rvc",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser()
                                        .$promise.then(res => {
                                            $rootScope.CurrentUser = res;
                                        });
                                } else {
                                    return $rootScope.CurrentUser;
                                }
                            }
                        ]
                    }
                })
                .state("vacation.approvereports",
                {
                    url: "/approve-reports",
                    templateUrl: "/App/Vacation/ApproveReports/ApproveReportsView.html",
                    controller: "Vacation.ApproveVacationController",
                    controllerAs: "avc",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser()
                                        .$promise.then(res => {
                                            $rootScope.CurrentUser = res;
                                        });
                                } else {
                                    return $rootScope.CurrentUser;
                                }
                            }
                        ]
                    }
                })
                .state("vacation.approvereportssettings",
                {
                    url: "/approve-reports-settings",
                    templateUrl: "/App/Core/Views/ApproveReportsSettingsView.html",
                    controller: "ApproveVacationReportsSettingsController",
                    controllerAs: "arsCtrl",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser()
                                        .$promise.then(res => {
                                            $rootScope.CurrentUser = res;
                                        });
                                } else {
                                    return $rootScope.CurrentUser;
                                }
                            }
                        ]
                    }
                })
                .state("vacation.admin",
                {
                    url: "/admin",
                    templateUrl: "/App/Vacation/Admin/AdminView.html",
                    controller: "Vacation.AdminMenuController",
                    controllerAs: "ctrl",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser()
                                        .$promise.then(res => {
                                            $rootScope.CurrentUser = res;
                                        });
                                } else {
                                    return $rootScope.CurrentUser;
                                }
                            }
                        ]
                    }
                })
                .state("vacation.myreports",
                {
                    url: "/myreports",
                    controller: 'MyVacationReportsController',
                    controllerAs: 'mvrc',
                    templateUrl: "/App/Vacation/MyVacationReports/MyVacationReportsView.html",
                    resolve: {
                        CurrentUser: [
                            "$rootScope", "Person", ($rootScope, Person) => {
                                if ($rootScope.CurrentUser == undefined) {
                                    return Person.GetCurrentUser()
                                        .$promise.then(res => {
                                            $rootScope.CurrentUser = res;
                                        });
                                } else {
                                    return $rootScope.CurrentUser;
                                }
                            }
                        ]
                    }
                })
                .state("vacation.myreports.pending",
                {
                    url: "/pending",
                    templateUrl:
                        "/App/Vacation/MyVacationReports/MyPendingVacationReports/MyPendingVacationReportsView.html",
                    controller: 'MyPendingVacationReportsController',
                    controllerAs: 'mvrCtrl'
                })
                .state("vacation.myreports.pending.edit",
                {
                    url: "/modal/:vacationReportId",
                    onEnter: [
                        "$state", "$modal", "$stateParams", ($state, $modal, $stateParams) => {
                            $modal.open({
                                    templateUrl: '/App/Vacation/MyVacationReports/EditVacationReportTemplate.html',
                                    controller: 'EditReportVacationController as rvc',
                                    backdrop: "static",
                                    windowClass: "app-modal-window-full",
                                    resolve: {
                                        vacationReportId: () => {
                                            return $stateParams.vacationReportId;
                                        }
                                    }
                                })
                                .result.then(() => {
                                    $state.go("^", null, { reload: true });
                                },
                                () => {
                                    $state.go("^");
                                });
                        }
                    ]
                })
                .state("vacation.myreports.approved",
                {
                    url: "/approved",
                    templateUrl: "/App/Vacation/MyVacationReports/MyApprovedVacationReports/MyApprovedVacationReportsView.html",
                    controller: 'MyApprovedVacationReportsController',
                    controllerAs: 'mvrCtrl'
                })
                .state("vacation.myreports.rejected",
                {
                    url: "/rejected",
                    templateUrl: "/App/Vacation/MyVacationReports/MyRejectedVacationReports/MyRejectedVacationReportsView.html",
                    controller: 'MyRejectedVacationReportsController',
                    controllerAs: 'mvrCtrl'
                });
        }
    }

    angular.module("app.vacation").config(Routes.init);
}
