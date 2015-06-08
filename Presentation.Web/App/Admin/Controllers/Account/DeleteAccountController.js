angular.module("application").controller("DeleteAccountController", [
    "$scope", "$modalInstance", "itemId", "NotificationService",
    function ($scope, $modalInstance, itemId, NotificationService) {

        $scope.confirmDelete = function () {
            /// <summary>
            /// Confirms deletion of BankAccount
            /// </summary>
            $modalInstance.close($scope.itemId);
            NotificationService.AutoFadeNotification("success", "", "Kontoen blev slettet.");
        }

        $scope.cancel = function () {
            /// <summary>
            /// Cancels deletion of BankAccount
            /// </summary>
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Sletning af kontoen blev annulleret.");
        }
    }
]);