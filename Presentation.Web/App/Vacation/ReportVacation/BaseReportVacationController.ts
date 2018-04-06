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
        purpose: String;
        careCpr: String;
        optionalText: String;
        position: number;
        saveButtenDisabled = true;
        isEditingVacation = false;
        startWeeks: number[] = [];
        endWeeks: number[] = [];
        startCalendarOptions: kendo.ui.CalendarOptions;
        endCalendarOptions: kendo.ui.CalendarOptions;
        vacationHours: number;
        vacationMinutes: number;
        freeVacationHours: number;
        freeVacationMinutes: number;
        children: any[] = [];
        child: string;

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
                this.calculateBalance();

                if (this.vacationBalances.length > 0) {
                    this.positionUpdated();
                    this.hasVacationBalance = true;
                } else {
                    this.hasVacationBalance = false;
                }

            });

            this.startCalendarOptions = {
                month: {
                    empty: '<a class="k-link disable-k-link"></a>'
                },
                navigate: (current) => {
                    var value = current.sender.current();
                    if (value != undefined) {
                        this.startWeeks = this.updateWeeks(value);
                        this.$scope.$apply();
                    }
                }
            };

            this.endCalendarOptions = {
                month: {
                    empty: '<span class="calendar-week-empty"> </span>'
                },
                navigate: (current) => {
                    var value = current.sender.current();
                    if (value != undefined) {
                        this.endWeeks = this.updateWeeks(value);
                        this.$scope.$apply();
                    }
                }
            };

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

        private updateWeeks(currentDate: Date): number[] {
            var m = moment(currentDate);
            var firstOfMonth = m.clone().startOf('month'),
                currentWeek = firstOfMonth.clone().day(0),
                output = [];

            if (firstOfMonth.isoWeekday() === 7 || firstOfMonth.isoWeekday() === 1) {
                output.push(currentWeek.isoWeek());
            }

            while (output.length < 6) {
                currentWeek.add(7, "d");
                output.push(currentWeek.isoWeek());
            }

            return output;
        }

        private sameMonth(a, b, other) {
            if (a.month() !== b.month()) {
                return other;
            }
            return a.date();
        }

        protected GetChildFromId(id) {
            var arrayLength = this.children.length;
            for (var i = 0; i < arrayLength; i++) {
                if (this.children[i].Id == id) {
                    return this.children[i];
                }
            }
            return null;
        }

        private updateCalendarRange() {
            if (this.startDate > this.endDate) {
                this.endDate = this.startDate;
            }
        }

        protected abstract initializeReport();

        timePickerOptions = {
            format: "HH:mm",
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

            (this.Person as any).GetChildren({ id: selectedPostion }).$promise.then(data => {
                this.children = data;
            });
        }

        childUpdated() {

        }

        abstract saveReport();

        clearReport() {
            this.initializeReport();
        }


        calculateBalance() {
            var totalVacation = this.vacationBalances[0].VacationHours + this.vacationBalances[0].TransferredHours;
            this.vacationHours = Math.floor(totalVacation);
            this.vacationMinutes = Math.round((totalVacation - this.vacationHours) * 60);
            this.freeVacationHours = Math.floor(this.vacationBalances[0].FreeVacationHours);
            this.freeVacationMinutes = Math.round((this.vacationBalances[0].FreeVacationHours - this.freeVacationHours) * 60);
        }
    }
}
