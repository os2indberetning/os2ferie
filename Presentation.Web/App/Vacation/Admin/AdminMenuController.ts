module app.vacation {
    "use strict";

    class AdminMenuController {

        static $inject: string[] = [
            "$scope",
            "Person",
            "$rootScope",
            "HelpText"
        ];

        private currentUser;

        constructor(private $scope, private Person, private $rootScope, private HelpText) {

            this.currentUser = $scope.CurrentUser;
        }

        orgSettingsClicked() {
            this.$scope.$broadcast("Vacation.OrgSettingsClicked");
        }

        adminClicked() {
            this.$scope.$broadcast("Vacation.AdministrationClicked");
        }

        reportsClicked() {
            this.$scope.$broadcast("Vacation.ReportsClicked");
        }

        accountClicked() {
            this.$scope.$broadcast("Vacation.AccountClicked");
        }

        emailClicked() {
            this.$scope.$broadcast("Vacation.EmailClicked");
        }
    }

    angular.module("app.vacation").controller("Vacation.AdminMenuController", AdminMenuController);
}
