angular.module("application").controller("DeleteMailNotificationController", [
    "$scope", "$modalInstance", "itemId", "NotificationService",
    function ($scope, $modalInstance, itemId, NotificationService) {

        $scope.confirmDelete = function () {
            /// <summary>
            /// Confirms deletion of MailNotification
            /// </summary>
            $modalInstance.close($scope.itemId);
            NotificationService.AutoFadeNotification("success", "", "Adviseringen blev slettet.");
        }
        


        $scope.cancel = function () {
            /// <summary>
            /// Cancels deletion of MailNotification
            /// </summary>
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Sletning af adviseringen blev annulleret.");
        }
    }
]);