angular.module("application").controller("AcceptController", [
   "$scope", "$modalInstance", "itemId", "NotificationService", "pageNumber", function ($scope, $modalInstance, itemId, NotificationService, pageNumber) {

       $scope.itemId = itemId;
       $scope.pageNumber = pageNumber;

       $scope.noClicked = function () {
           $modalInstance.dismiss('cancel');
           NotificationService.AutoFadeNotification("warning", "Annuller", "Godkendelsen blev annulleret.");
       }

       $scope.yesClicked = function () {
           $modalInstance.close($scope.itemId);
           NotificationService.AutoFadeNotification("success", "Godkendt", "Indberetningen blev godkendt.");
       }

       $scope.approveSelectedClick = function () {
           $modalInstance.close();
           NotificationService.AutoFadeNotification("success", "Godkendt", "De markerede indberetninger blev godkendt.");
       }

   }
]);