module app.vacation {
    "use strict";

    import VacationBalanceResource = vacation.resources.IVacationBalanceResource;
    import Balance = core.models.VacationBalance;
    import Report = core.models.VacationReport;
    import Employment = core.models.Employment;
    import Person = core.models.Person;
    import NotificationService = core.interfaces.NotificationService;

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
            "$modalInstance",
            "vacationReportId"
        ];

        vacationBalances: Balance[];
        vacationBalance: Balance;
        hasVacationBalance: Boolean;
        vacationDaysInPeriod: number;
        vacationStartsOnFullDay = true;
        vacationEndsOnFullDay = true;
        vacationReport: Report;
        isEditingReport: boolean;
        mimimumVacationStartDate = new Date(2016, 4, 1); // Can't report vacation before the system was launched. Might be changed later.
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
        private currentUser: Person;

        constructor(
            private $scope,
            private Person: Person,
            private $rootScope,
            private VacationReport, // TODO Make $resource class
            private NotificationService: NotificationService,
            private VacationBalanceResource: VacationBalanceResource,
            private moment: moment.MomentStatic,
            private $modal: angular.ui.bootstrap.IModalService,
            private $modalInstance: angular.ui.bootstrap.IModalServiceInstance,
            private vacationReportId: number) {

            this.currentUser = $scope.CurrentUser;

            this.isEditingReport = vacationReportId !== 0;

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
                this.updateCalendarRange();
            });

            this.employments = [];

            angular.forEach(this.currentUser.Employments, (value: any) => {
                value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription + " (" + value.EmploymentId + ")";
                if (value.OrgUnit.HasAccessToVacation) this.employments.push(value);
            });

            this.initializeReport();
        }

        private updateCalendarRange() {
            if (this.startDate > this.endDate) {
                this.endDate = this.startDate;
            }
        }

        private initializeReport() {
            if (!this.isEditingReport) {
                this.startDate = new Date();
                this.endDate = new Date();
                this.maxEndDate = new Date();
                this.comment = undefined;
                this.vacationStartsOnFullDay = true;
                this.vacationEndsOnFullDay = true;
                this.startTime = new Date(2000, 0, 1, 0, 0, 0, 0); // Only time is relevant, date is ignored by kendo
                this.endTime = new Date(2000, 0, 1, 0, 0, 0, 0);
                this.vacationType = undefined;
            } else {
                var report = this.VacationReport.get({ id: this.vacationReportId }, () => {
                    console.log(report);

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

                    this.comment = report.Comment;
                    this.vacationType = report.VacationType;
                    this.position = report.EmploymentId;
                });
            }
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
            const report = new this.VacationReport();

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
            } else {
                report.StartTime = null;
            }

            if (!this.vacationEndsOnFullDay) {
                report.EndTime = `P0DT${this.endTime.getHours()}H${this.endTime.getMinutes()}M0S`;
            } else {
                report.EndTime = null;
            }

            if (this.isEditingReport) {
                report.Id = this.vacationReportId;

                report.$update({id: this.vacationReportId},() => {
                    this.NotificationService
                        .AutoFadeNotification("success", "", "Din indberetning er blevet rédigeret.");
                    this.$modalInstance.close();
                },
                    () => {
                        this.saveButtenDisabled = false;
                        this.NotificationService
                            .AutoFadeNotification("danger",
                            "",
                            "Der opstod en fejl under rédigering af din ferieindberetning.");
                    });

            } else {
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
                            "Der opstod en fejl under oprettelsen af din ferieindberetning.");
                });
            }

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
