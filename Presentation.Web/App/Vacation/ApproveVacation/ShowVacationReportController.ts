module app.vacation {
    "use strict";

    import NotificationService = core.interfaces.NotificationService;
    import VacationReport = core.models.VacationReport;

    class ShowVacationReportController {

        static $inject = [
            "$scope",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "moment",
            "$state",
            "$modalInstance",
            "$modal",
            "report"
        ];

        name: string;
        start: string;
        end: string;
        startTime: string;
        endTime: string;
        purpose: string;
        type: string;
        status: string;

        loadingPromise;

        constructor(private $scope,
            private $rootScope,
            private VacationReport,
            private NotificationService: NotificationService,
            private moment: moment.MomentStatic,
            private $state: ng.ui.IStateService,
            private $modalInstance,
            private $modal,
            public report) {

            this.name = report.Person.FullName.split("[")[0];
            this.purpose = report.description;

            const startsOnFullDay = report.StartTime == null;
            const endsOnFullDay = report.EndTime == null;

            if (!startsOnFullDay) {
                this.startTime = "Fra kl. " + moment((moment.duration(report.StartTime) as any)._data).format('HH:mm');
            } else {
                this.startTime = "Hele dagen";
            }

            if (!endsOnFullDay) {
                this.endTime = "Til kl. " + moment((moment.duration(report.EndTime) as any)._data).format('HH:mm');
            } else {
                this.endTime = "Hele dagen";
                report.end -= 86400;
            }

            this.start = moment(report.start).format("DD.MM.YYYY");
            this.end = moment(report.end).format("DD.MM.YYYY");

            this.type = report.type === "Regular" ? "Almindelig ferie" : "6. ferieuge";

            switch (this.report.status) {
                case "Accepted":
                    this.status = "Godkendt";
                    break;
                case "Rejected":
                    this.status = "Afvist";
                    break;
                case "Pending":
                    this.status = "Afventende";
                    break;
                default:
                    this.status = "Ukendt";
                    break;
            }

        }

        close() {
            this.$modalInstance.dismiss();
        }

        approve() {
            var report = new this.VacationReport();
            this.loadingPromise = report.$approve({ id: this.report.id },
                () => {
                    this.NotificationService.AutoFadeNotification("success", "", "Indberetningen blev godkendt.");
                    this.$modalInstance.close();
                });
        }

        reject() {
            this.$modal.open({
                templateUrl: '/App/Core/Views/Modals/ConfirmRejectReport.html',
                controller: 'RejectReportModalInstanceController',
                backdrop: "static",
                resolve: {
                    itemId: () => {
                        return this.report.id;
                    }
                }
            }).result.then(res => {
                var report = new this.VacationReport();
                this.loadingPromise = report.$reject({ id: this.report.id, comment: res.Comment },
                    () => {
                        this.NotificationService.AutoFadeNotification("success", "", "Indberetningen blev afvist.");
                        this.$modalInstance.close();
                    });
            });
        }
    }

    angular.module("app.vacation").controller("ShowVacationReportController", ShowVacationReportController);
}
