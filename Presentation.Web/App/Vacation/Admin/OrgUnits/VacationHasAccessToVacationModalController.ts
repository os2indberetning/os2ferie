module app.vacation {
    "use strict";

    import NotificationService = core.interfaces.NotificationService;
    import VacationBalanceResource = vacation.resources.IVacationBalanceResource;
    import VacationReport = core.models.VacationReport;
    import Balance = core.models.VacationBalance;

    class VacationHasAccessToVacationModalController {

        static $inject = [
            "$scope",
            "$modalInstance",
            "$modal",
        ];
        
        constructor(private $scope,
            private $modalInstance,
            private $modal) {

        }

        close() {
            this.$modalInstance.close(false);
        }

        all() {
           this.$modalInstance.close(true);
        }
        
    }

    angular.module("app.vacation").controller("VacationHasAccessToVacationModalController", VacationHasAccessToVacationModalController);
}
