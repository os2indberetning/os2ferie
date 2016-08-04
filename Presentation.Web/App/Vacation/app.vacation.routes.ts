module app.vacation {

    export class Routes {
        static $inject = [
            "$stateProvider"
        ];

        static init($stateProvider: ng.ui.IStateProvider) {
            $stateProvider
                .state("vacation.report",
                {
                    url: "/report",
                    templateUrl: "/App/Vacation/ReportVacation/ReportVacationView.html",
                    controller: "CreateReportVacationController",
                    controllerAs: "rvc"
                })
                .state("vacation.approvereports",
                {
                    url: "/approve-reports",
                    templateUrl: "/App/Vacation/ApproveVacation/ApproveVacationView.html",
                    controller: "ApproveVacationController",
                    controllerAs: "avc",
                    data: {
                        roles: ['Approver']
                    }
                })
                .state("vacation.approvereportssettings",
                {
                    url: "/approve-reports-settings",
                    templateUrl: "/App/Core/Views/ApproveReportsSettingsView.html",
                    controller: "ApproveVacationSettingsController",
                    controllerAs: "arsCtrl",
                    data: {
                        roles: ['Approver']
                    }
                })
                .state("vacation.admin",
                {
                    url: "/admin",
                    templateUrl: "/App/Vacation/Admin/AdminView.html",
                    controller: "Vacation.AdminMenuController",
                    controllerAs: "ctrl",
                    data: {
                        roles: ['Admin']
                    }
                })
                .state("vacation.myreports",
                {
                    url: "/myreports",
                    controller: 'MyVacationReportsController',
                    controllerAs: 'mvrc',
                    templateUrl: "/App/Vacation/MyVacationReports/MyVacationReportsView.html"
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
                .state("vacation.myreports.approved.edit",
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
