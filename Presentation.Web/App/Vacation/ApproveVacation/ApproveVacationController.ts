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
                    heading: 'Stedfortrædere',
                    route: 'vacation.approve.settings'
                }
            ];
        }
    }

    angular.module("app.vacation").controller("ApproveVacationController", ApproveVacationController);
}
