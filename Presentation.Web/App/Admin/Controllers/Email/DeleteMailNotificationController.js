angular.module("application").controller("DeleteMailNotificationController", [
    "$scope", "$modalInstance", "itemId", "NotificationService",
    function ($scope, $modalInstance, itemId, NotificationService) {

     $scope.confirmDelete = function () {
            $modalInstance.close($scope.itemId);
            NotificationService.AutoFadeNotification("success", "Slet", "Adviseringen blev slettet.");
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "Slet", "Sletning af adviseringen blev annulleret.");
        }
    }
]);