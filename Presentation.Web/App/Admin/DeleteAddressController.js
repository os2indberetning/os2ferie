angular.module("application").controller("DeleteAddressController", [
    "$scope", "$modalInstance", "itemId", "NotificationService", function ($scope, $modalInstance, itemId, NotificationService) {

        $scope.itemId = itemId;

        $scope.confirmDelete = function () {
            $modalInstance.close($scope.itemId);
            NotificationService.AutoFadeNotification("success", "Slet", "Adressen blev slettet.");
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "Slet", "Sletning af adressen blev annulleret.");
        }
    }
]);