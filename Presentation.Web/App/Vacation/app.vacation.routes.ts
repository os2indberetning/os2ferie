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
                        adminEditCurrentUser() { return 0; },
                        ReportId() { return -1; },
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
                    controller: "Vacation.ApproveReportsController",
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
                        ],
                    }
                });
        }
    }

    angular.module("app.vacation").config(Routes.init);
}