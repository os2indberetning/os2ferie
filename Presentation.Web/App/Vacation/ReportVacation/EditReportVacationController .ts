module app.vacation {
    "use strict";

    import VacationBalanceResource = vacation.resources.IVacationBalanceResource;
    import Person = core.models.Person;
    import NotificationService = core.interfaces.NotificationService;

    class EditReportVacationController extends BaseReportVacationController {

        static $inject = [
            "$scope",
            "Person",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "VacationBalanceResource",
            "moment",
            "$modal",
            "$modalInstance",
            "vacationReportId"
        ];

        constructor(
            protected $scope,
            protected Person: Person,
            protected $rootScope,
            protected VacationReport, // TODO Make $resource class
            protected NotificationService: NotificationService,
            protected VacationBalanceResource: VacationBalanceResource,
            protected moment: moment.MomentStatic,
            protected $modal: angular.ui.bootstrap.IModalService,
            protected $modalInstance: angular.ui.bootstrap.IModalServiceInstance,
            private vacationReportId: number) {

            super($scope, Person, $rootScope, VacationReport, NotificationService, VacationBalanceResource, moment);

            this.initializeReport();
            this.isEditingReport = true;
        }

        protected initializeReport() {

            var report = this.VacationReport.get({ id: this.vacationReportId },
            () => {
                this.startDate = this.moment.utc(report.StartTimestamp, "X").toDate();
                this.endDate = this.moment.utc(report.EndTimestamp, "X").toDate();

                this.vacationStartsOnFullDay = report.StartTime == null;
                this.vacationEndsOnFullDay = report.EndTime == null;

                if (!this.vacationStartsOnFullDay) {
                    const date = new Date();
                    const duration = this.moment.duration(report.StartTime);
                    date.setHours(duration.hours());
                    date.setMinutes(duration.minutes());
                    this.startTime = date;
                }

                if (!this.vacationEndsOnFullDay) {
                    const date = new Date();
                    const duration = this.moment.duration(report.EndTime);
                    date.setHours(duration.hours());
                    date.setMinutes(duration.minutes());
                    this.endTime = date;
                }

                this.purpose = report.Purpose;
                this.vacationType = report.VacationType;
                this.position = report.EmploymentId;
            });
        }

        saveReport() {
            const report = new this.VacationReport();

            report.StartTimestamp = this.moment(this.startDate).unix();
            report.EndTimestamp = this.moment(this.endDate).unix();
            report.EmploymentId = this.position;
            report.Purpose = this.purpose;
            report.PersonId = this.currentUser.Id;
            report.Status = "Pending";
            report.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
            report.VacationType = this.vacationType;

            if (!this.vacationStartsOnFullDay) {
                report.StartTime = `P0DT${this.startTime.getHours()}H${this.startTime.getMinutes()}M0S`;
            } else {
                report.StartTime = null;
            }

            if (!this.vacationEndsOnFullDay) {
                report.EndTime = `P0DT${this.endTime.getHours()}H${this.endTime.getMinutes()}M0S`;
            } else {
                report.EndTime = null;
            }

            report.Id = this.vacationReportId;

            report.$update({ id: this.vacationReportId },
            () => {
                this.NotificationService
                    .AutoFadeNotification("success", "", "Din indberetning er blevet rédigeret.");
                this.$modalInstance.close();
            },
            () => {
                this.saveButtenDisabled = false;
                this.NotificationService
                    .AutoFadeNotification("danger",
                        "",
                        "Der opstod en fejl under rédigering af din ferieindberetning (Holder du allerede ferie i den valgte periode?).");
            });

        }

        closeModalWindow() {
            this.$modalInstance.dismiss();
        }
    }

    angular.module("app.vacation").controller("EditReportVacationController", EditReportVacationController);
}
