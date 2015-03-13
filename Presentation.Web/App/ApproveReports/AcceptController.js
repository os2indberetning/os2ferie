angular.module("application").controller("AcceptController", [
   "$scope", "$modalInstance", "itemId", "NotificationService", function ($scope, $modalInstance, itemId, NotificationService) {

        $scope.itemId = itemId;

       $scope.noClicked = function () {
           $modalInstance.dismiss('cancel');
           NotificationService.AutoFadeNotification("warning", "Annuller", "Godkendelsen af indberetningen blev annulleret.");
       }

       $scope.yesClicked = function () {
           $modalInstance.close($scope.itemId);
           NotificationService.AutoFadeNotification("success", "Godkendt", "Indberetningen blev godkendt.");
       }

   }
]);