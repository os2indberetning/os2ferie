module app.vacation {
    "use strict";

    class MyVacationReportsController {

        static $inject: string[] = [];

        tabData;

        constructor() {
            this.tabData = [
                {
                    heading: 'Afventer',
                    route: 'vacation.myreports.pending'
                },
                {
                    heading: 'Godkendte',
                    route: 'vacation.myreports.approved'
                },
                {
                    heading: 'Afviste',
                    route: 'vacation.myreports.rejected'
                }
            ];
        }
    }

    angular.module("app.vacation").controller("MyVacationReportsController", MyVacationReportsController);
}
