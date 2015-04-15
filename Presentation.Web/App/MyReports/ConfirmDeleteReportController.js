angular.module("application").controller("ConfirmDeleteReportController", [
    "$scope", "$modalInstance", "itemId", "NotificationService", function ($scope, $modalInstance, itemId, NotificationService) {

        $scope.itemId = itemId;

        $scope.confirmDelete = function() {
            $modalInstance.close($scope.itemId);
            NotificationService.AutoFadeNotification("success", "", "Indberetningen blev slettet.");
        }

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Sletning af indberetningen blev annulleret.");
        }
    }
]);