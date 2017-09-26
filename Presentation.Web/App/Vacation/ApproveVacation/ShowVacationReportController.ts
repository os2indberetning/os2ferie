module app.vacation {
    "use strict";

    import NotificationService = core.interfaces.NotificationService;
    import VacationBalanceResource = vacation.resources.IVacationBalanceResource;
    import VacationReport = core.models.VacationReport;
    import Balance = core.models.VacationBalance;

    class ShowVacationReportController {

        static $inject = [
            "$scope",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "VacationBalanceResource",
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
        optionalText: string;
        type: string;
        status: string;
        id: number;
        vacationBalance: Balance;
        vacationHours: number;
        vacationMinutes: number;
        freeVacationHours: number;
        freeVacationMinutes: number;
        payDeduction: string;

        loadingPromise;

        vacationTime: number;

        constructor(private $scope,
            private $rootScope,
            private VacationReport,
            private NotificationService: NotificationService,
            private VacationBalanceResource: VacationBalanceResource,
            private moment: moment.MomentStatic,
            private $state: ng.ui.IStateService,
            private $modalInstance,
            private $modal,
            public report) {

            this.id = report.personId;

            VacationBalanceResource.forEmployment({ id: report.EmploymentId }).$promise.then(data => {
                this.vacationBalance = data;
                this.calculateBalance();
                this.calculatePayDeduction();
            })

            this.name = report.Person.FullName.split("[")[0];
            this.purpose = report.description;

            const startsOnFullDay = report.StartTime == null;
            const endsOnFullDay = report.EndTime == null;

            this.vacationTime = moment(report.end).diff(moment(report.start), 'days') * 7.5;

            if (!startsOnFullDay) {
                this.startTime = "Fra kl. " + moment((moment.duration(report.StartTime) as any)._data).format('HH:mm');
            } else {
                this.startTime = "Hele dagen";
            }

            if (!endsOnFullDay) {
                this.endTime = "Til kl. " + moment((moment.duration(report.EndTime) as any)._data).format('HH:mm');
            } else {
                this.endTime = "Hele dagen";
            }

            this.start = moment(report.start).format("DD.MM.YYYY");
            this.end = moment(report.end).format("DD.MM.YYYY");

            switch (report.type) {
                case "Care":
                    this.type = "Omsorgsdage";
                    this.optionalText = report.OptionalText;
                    break;
                case "Optional":
                    this.type = "Andet fravær";
                    this.optionalText = report.OptionalText;
                    break;
                case "Regular":
                    this.type = "Almindelig Ferie";
                    break;
                case "Senior":
                    this.type = "Seniordage";
                    break;
                case "SixthVacationWeek":
                    this.type = "6. ferieuge";
                    break;
                default:
                    this.type = "Andet";
                    break;
            }

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
                },
                (err) => {
                    console.log(err);
                    if (err.data.error.message == null) {
                        this.NotificationService.AutoFadeNotification("danger", "", "Der skete en ukendt fejl");
                    }
                    else {
                        var message = err.data.error.message;
                        this.NotificationService.AutoFadeNotification("danger", "", message);
                    }
                }
            );
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

        calculateBalance() {
            var totalVacation = this.vacationBalance.VacationHours + this.vacationBalance.TransferredHours;
            this.vacationHours = Math.floor(totalVacation);
            this.vacationMinutes = Math.round((totalVacation - this.vacationHours) * 60);
            this.freeVacationHours = Math.floor(this.vacationBalance.FreeVacationHours);
            this.freeVacationMinutes = Math.round((this.vacationBalance.FreeVacationHours - this.freeVacationHours) * 60);
        }

        calculatePayDeduction() {
            var payDeduction = false;
            if (this.report.status === "Pending") {
                var totalVacationHours = this.calculateTotalVacationHours();
                switch (this.report.type) {
                    case "Care":
                        break;
                    case "Optional":
                        break;
                    case "Regular":
                        payDeduction = totalVacationHours < this.vacationTime;
                        break;
                    case "Senior":
                        break;
                    case "SixthVacationWeek":
                        payDeduction = totalVacationHours < this.vacationTime;
                        break;
                    default:
                        break;
                }
            }

            if (payDeduction) {
                this.payDeduction = "Ja";
            } else {
                this.payDeduction = "Nej";
            }
        }

        private calculateTotalVacationHours(): number {
            return this.report.VacationHours;
        }
    }

    angular.module("app.vacation").controller("ShowVacationReportController", ShowVacationReportController);
}
