module app.vacation {
    "use strict";

    import VacationBalanceResource = vacation.resources.IVacationBalanceResource;
    import Balance = core.models.VacationBalance;
    import Report = core.models.VacationReport;
    import Employment = app.core.models.Employment;

    class ReportVacationController {
        static $inject = [
            "$scope",
            "Person",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "VacationBalanceResource",
            "moment",
            "$modal",
            "$modalInstance"
        ];

        vacationBalances: Balance[];
        vacationBalance: Balance;
        hasVacationBalance: Boolean;
        vacationDaysInPeriod: number;

        vacationStartsOnFullDay = true;
        vacationEndsOnFullDay = true;

        vacationReport: Report;

        startDate: Date;
        endDate: Date;
        startTime: Date;
        endTime: Date;
        employments: Employment[];
        vacationType;
        comment: String;
        position: number;
        saveButtenDisabled = true;

        private maxEndDate: Date;
        private currentUser;

        constructor(private $scope, private Person, private $rootScope, private VacationReport, private NotificationService, private VacationBalanceResource: VacationBalanceResource, private moment, private $modal, private $modalInstance) {

            this.currentUser = $scope.CurrentUser;

            VacationBalanceResource.query().$promise.then(data => {
                this.vacationBalances = data;

                if (this.vacationBalances.length > 0) {
                    this.positionUpdated();
                    this.hasVacationBalance = true;
                } else {
                    this.hasVacationBalance = false;
                }

            });

            this.vacationDaysInPeriod = 0;

            this.$scope.$watch(() => { return this.startDate }, () => {
                if (this.startDate > this.endDate) {
                    this.endDate = this.startDate;
                } else {
                    this.calculateVacationPeriod(this.startDate, this.endDate);
                }
            });

            this.$scope.$watch(() => { return this.endDate }, () => {
                this.calculateVacationPeriod(this.startDate, this.endDate);
            });


            this.employments = [];

            angular.forEach(this.currentUser.Employments, (value) => {
                value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription + " (" + value.EmploymentId + ")";
                if (value.OrgUnit.HasAccessToVacation) this.employments.push(value);
            });

            this.initializeReport();
        }

        private calculateVacationPeriod(start, end) {
            this.vacationDaysInPeriod = Math.abs(end - start) / 1000 / 60 / 60;
            // this.netVacationRemainingIfApproved = this.netVacationRemaining - this.vacationDaysInPeriod;
        }

        private initializeReport() {
            this.startDate = new Date();
            this.endDate = new Date();
            this.maxEndDate = new Date();
            this.comment = undefined;
            this.vacationStartsOnFullDay = true;
            this.vacationEndsOnFullDay = true;
            this.startTime = new Date(2000, 0, 1, 0, 0, 0, 0); // Only time is relevant, date is ignored by kendo
            this.endTime = new Date(2000, 0, 1, 0, 0, 0, 0);
            this.vacationType = undefined;
        }

        timePickerOptions: kendo.ui.TimePickerOptions = {
            interval: 15,
            value: new Date(2000, 0, 1, 0, 0, 0, 0)
        }

        positionUpdated() {
            const selectedPostion = this.position;
            const vacationBalances = this.vacationBalances;
            for (let v in vacationBalances) {
                if (vacationBalances.hasOwnProperty(v)) {
                    const vb = vacationBalances[v];

                    if (vb.EmploymentId == selectedPostion) {
                        this.vacationBalance = vb;
                    }
                }
            }
        }

        saveReport() {
            var report = new this.VacationReport();


            report.StartTimestamp = Math.floor((new Date(this.startDate.getFullYear(), this.startDate.getMonth(), this.startDate.getDate()).getTime()) / 1000);
            report.EndTimestamp = Math.floor((new Date(this.endDate.getFullYear(), this.endDate.getMonth(), this.endDate.getDate()).getTime()) / 1000);
            report.EmploymentId = this.position;
            report.Comment = this.comment;

            report.PersonId = this.currentUser.Id;
            report.Status = "Pending";
            report.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
            report.VacationType = this.vacationType;

            if (!this.vacationStartsOnFullDay) {
                report.StartTime = `P0DT${this.startTime.getHours()}H${this.startTime.getMinutes()}M0S`;
            }

            if (!this.vacationEndsOnFullDay) {
                report.EndTime = `P0DT${this.endTime.getHours()}H${this.endTime.getMinutes()}M0S`;
            }

            report.$save(res => {
                this.$scope.latestDriveReport = res;

                this.NotificationService.AutoFadeNotification("success", "", "Din indberetning er sendt til godkendelse.");

                this.clearReport();
                this.saveButtenDisabled = false;
            }, () => {
                this.saveButtenDisabled = false;
                this.NotificationService.AutoFadeNotification("danger", "", "Der opstod en fejl under oprettelsen af din ferieindberetning.");
            });
        }

        clearReport() {
            this.initializeReport();
        }

        closeModalWindow() {
            this.$modalInstance.dismiss();
        }

    }

    angular.module("app.vacation").controller("ReportVacationController", ReportVacationController);
}
