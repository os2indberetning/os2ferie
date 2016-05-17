module app.vacation {
    "use strict";

    class ConfirmDeleteVacationReportController {

        static $inject: string[] = [
            "$modalInstance",
            "itemId",
            "NotificationService"
        ];

        constructor(private $modalInstance, public itemId, private NotificationService) {
            this.itemId = itemId;
        }

        confirmDelete() {
            this.$modalInstance.close(this.itemId);
            this.NotificationService.AutoFadeNotification("success", "", "Indberetningen blev slettet.");
        }

        cancel() {
            this.$modalInstance.dismiss('cancel');
            this.NotificationService.AutoFadeNotification("warning", "", "Sletning af indberetningen blev annulleret.");
        }

    }

    angular.module("app.vacation").controller("ConfirmDeleteVacationReportController", ConfirmDeleteVacationReportController);
}