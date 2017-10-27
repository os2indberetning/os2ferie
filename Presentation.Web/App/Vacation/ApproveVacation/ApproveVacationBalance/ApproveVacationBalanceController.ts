module app.vacation {
    "use strict";

    import VacationBalanceResource = vacation.resources.IVacationBalanceResource;

    class ApproveVacationBalanceController {

        static $inject = [
            "$http",
            "VacationBalanceResource"
        ];

        persons;
        isReady = false;

        constructor(
            protected $http,
            protected VacationBalanceResource: VacationBalanceResource
        ) {
            $http.get("/odata/Person/Service.LeadersPeople(Type=1)").then((response: ng.IHttpPromiseCallbackArg<any>) => {
                this.persons = response.data.value;
                angular.forEach(this.persons, (value, key) => {
                    this.getVacationBalance(value.Id, key);
                });
            });
        }

        getVacationBalance(id: number, idx: number) {
            this.VacationBalanceResource.forEmployee({ id: id }).$promise.then(data => {
                this.calculateBalance(data, idx);
                if (idx === this.persons.length - 1) {
                    this.isReady = true;
                }
            });
        }

        calculateBalance(data: any, idx: number) {
            var vacationHours, vacationMinutes, freeVacationHours, freeVacationMinutes;

            if (data) {
                var totalVacation = data.VacationHours + data.TransferredHours;
                vacationHours = Math.floor(totalVacation);
                vacationMinutes = Math.round((totalVacation - vacationHours) * 60);
                freeVacationHours = Math.floor(data.FreeVacationHours);
                freeVacationMinutes = Math.round((data.FreeVacationHours - freeVacationHours) * 60);
            } else {
                vacationHours = 0;
                vacationMinutes = 0;
                freeVacationHours = 0;
                freeVacationMinutes = 0;
            }
            this.persons[idx].VacationBalance = { vacationHours, vacationMinutes, freeVacationHours, freeVacationMinutes };
        }
    }

    angular.module("app.vacation").controller("ApproveVacationBalanceController", ApproveVacationBalanceController);
}
