module app.vacation {
    "use strict";

    class ApproveVacationController {

        static $inject: string[] = [];

        tabData;

        constructor() {
            this.tabData = [
                {
                    heading: 'Godkend',
                    route: 'vacation.approve.pending'
                },
                {
                    heading: 'Ferie saldo',
                    route: 'vacation.approve.balance'
                },
                {
                    heading: 'Stedfortrædere',
                    route: 'vacation.approve.settings'
                }
            ];
        }
    }

    angular.module("app.vacation").controller("ApproveVacationController", ApproveVacationController);
}
