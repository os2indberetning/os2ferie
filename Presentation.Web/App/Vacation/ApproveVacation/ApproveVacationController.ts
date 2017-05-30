module app.vacation {
    "use strict";

    class ApproveVacationController {

        static $inject: string[] = [];

        tabData;

        constructor() {
            this.tabData = [
                {
                    heading: 'Overblik',
                    route: 'vacation.approve.pending'
                },
                {
                    heading: 'Feriesaldo',
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
