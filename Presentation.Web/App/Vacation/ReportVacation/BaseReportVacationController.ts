module app.vacation {
    "use strict";

    import VacationBalanceResource = vacation.resources.IVacationBalanceResource;
    import Balance = core.models.VacationBalance;
    import Report = core.models.VacationReport;
    import Employment = core.models.Employment;
    import Person = core.models.Person;
    import NotificationService = core.interfaces.NotificationService;

    export abstract class BaseReportVacationController {
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
        isEditingVacation = false;

        protected maxEndDate: Date;
        protected currentUser: Person;

        constructor(
            protected $scope,
            protected Person: Person,
            protected $rootScope,
            protected VacationReport, // TODO Make $resource class
            protected NotificationService: NotificationService,
            protected VacationBalanceResource: VacationBalanceResource,
            protected moment: moment.MomentStatic) {

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
                this.updateCalendarRange();
            });

            this.employments = [];

            angular.forEach(this.currentUser.Employments, (value: any) => {
                value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription + " (" + value.EmploymentId + ")";
                if (value.OrgUnit.HasAccessToVacation) this.employments.push(value);
            });
        }

        private updateCalendarRange() {
            if (this.startDate > this.endDate) {
                this.endDate = this.startDate;
            }
        }

        protected abstract initializeReport();

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

        abstract saveReport();

        clearReport() {
            this.initializeReport();
        }
    }
}
