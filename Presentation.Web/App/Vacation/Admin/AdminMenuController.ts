module app.vacation {
    "use strict";

    class AdminMenuController {

        static $inject = [
            "$scope",
            "Person",
            "$rootScope",
            "HelpText"
        ];

        private _currentUser;

        constructor(private $scope, private Person, private $rootScope, private HelpText) {

            this._currentUser = $scope.CurrentUser;
        }

        OrgSettingsClicked() {
            this.$scope.$broadcast("Vacation.OrgSettingsClicked");
        }

        AdminClicked() {
            this.$scope.$broadcast("Vacation.AdministrationClicked");
        }

        ReportsClicked() {
            this.$scope.$broadcast("Vacation.ReportsClicked");
        }

        AccountClicked() {
            this.$scope.$broadcast("Vacation.AccountClicked");
        }

        EmailClicked() {
            this.$scope.$broadcast("Vacation.EmailClicked");
        }
    }

    angular.module("app.vacation").controller("Vacation.AdminMenuController", AdminMenuController);
}