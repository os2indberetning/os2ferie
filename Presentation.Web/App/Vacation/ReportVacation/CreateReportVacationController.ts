module app.vacation {
    "use strict";

    import VacationBalanceResource = vacation.resources.IVacationBalanceResource;
    import Person = core.models.Person;
    import NotificationService = core.interfaces.NotificationService;

    class CreateReportVacationController extends BaseReportVacationController {

        static $inject = [
            "$scope",
            "Person",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "VacationBalanceResource",
            "moment"
        ];

        constructor(
            protected $scope,
            protected Person: Person,
            protected $rootScope,
            protected VacationReport, // TODO Make $resource class
            protected NotificationService: NotificationService,
            protected VacationBalanceResource: VacationBalanceResource,
            protected moment: moment.MomentStatic) {

            super($scope, Person, $rootScope, VacationReport, NotificationService, VacationBalanceResource, moment);

            this.initializeReport();
        }

        protected initializeReport() {
            this.startDate = new Date();
            this.endDate = new Date();
            this.maxEndDate = new Date();
            this.purpose = undefined;
            this.vacationStartsOnFullDay = true;
            this.vacationEndsOnFullDay = true;
            this.startTime = new Date(2000, 0, 1, 0, 0, 0, 0); // Only time is relevant, date is ignored by kendo
            this.endTime = new Date(2000, 0, 1, 0, 0, 0, 0);
            this.vacationType = undefined;
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

            report.$save(() => {
                this.NotificationService
                    .AutoFadeNotification("success", "", "Din indberetning er sendt til godkendelse.");

                this.clearReport();
                this.saveButtenDisabled = false;
            },
            () => {
                this.saveButtenDisabled = false;
                this.NotificationService
                    .AutoFadeNotification("danger",
                        "",
                        "Der opstod en fejl under oprettelsen af din ferieindberetning (Holder du allerede ferie i den valgte periode?).");
            });
        }
    }

    angular.module("app.vacation").controller("CreateReportVacationController", CreateReportVacationController);
}
