angular.module("app.drive").controller("AcceptController", [
   "$scope", "$modalInstance", "itemId", "NotificationService" +
    "", "pageNumber", function ($scope, $modalInstance, itemId, NotificationService, pageNumber) {

       $scope.itemId = itemId;
       $scope.pageNumber = pageNumber;

       $scope.noClicked = function () {
           $modalInstance.dismiss('cancel');
           NotificationService.AutoFadeNotification("warning", "", "Godkendelsen blev annulleret.");
       }

       $scope.yesClicked = function () {
           $modalInstance.close($scope.itemId);
           NotificationService.AutoFadeNotification("success", "", "Indberetningen blev godkendt.");
       }

       $scope.approveSelectedClick = function () {
           $modalInstance.close();
           NotificationService.AutoFadeNotification("success", "", "De markerede indberetninger blev godkendt.");
       }

   }
]);