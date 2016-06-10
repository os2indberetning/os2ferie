module app.vacation {
    "use strict";

    import NotificationService = core.interfaces.NotificationService;

    class ConfirmDeleteVacationReportController {

        static $inject: string[] = [
            "$modalInstance",
            "itemId",
            "NotificationService"
        ];

        constructor(private $modalInstance: angular.ui.bootstrap.IModalServiceInstance,
            public itemId: number,
            private NotificationService: NotificationService) {
            this.itemId = itemId;
        }

        confirmDelete() {
            this.$modalInstance.close(this.itemId);
        }

        cancel() {
            this.$modalInstance.dismiss('cancel');
            this.NotificationService.AutoFadeNotification("warning", "", "Sletning af indberetningen blev annulleret.");
        }
    }

    angular.module("app.vacation").controller("ConfirmDeleteVacationReportController", ConfirmDeleteVacationReportController);
}
