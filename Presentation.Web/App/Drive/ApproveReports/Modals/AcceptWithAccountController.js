angular.module("app.drive").controller("AcceptWithAccountController", [
   "$scope", "$modalInstance", "itemId", "BankAccount", "NotificationService", "pageNumber", function ($scope, $modalInstance, itemId, BankAccount, NotificationService, pageNumber) {

       $scope.itemId = itemId;

       $scope.result = {};

       $scope.pageNumber = pageNumber;

       BankAccount.get().$promise.then(function (res) {
           $scope.accounts = res.value;
       });

       $scope.noClicked = function () {
           $modalInstance.dismiss('cancel');
           NotificationService.AutoFadeNotification("warning", "", "Godkendelsen af indberetningen blev annulleret.");
       }

       $scope.yesClicked = function () {
           if ($scope.selectedAccount == undefined) {
               $scope.errorMessage = "* Du skal vælge en konto";
           } else {
               $scope.result.AccountNumber = $scope.selectedAccount.Number;
               $scope.result.Id = itemId;
               $modalInstance.close($scope.result);
               NotificationService.AutoFadeNotification("success", "", "Indberetningen blev godkendt med kontering " + $scope.selectedAccount.Description + " - " + $scope.selectedAccount.Number);
           }
       }

       $scope.approveAllWithAccountClick = function () {
           if ($scope.selectedAccount == undefined) {
               $scope.errorMessage = "* Du skal vælge en konto";
           } else {
               $modalInstance.close($scope.selectedAccount.Number);
               NotificationService.AutoFadeNotification("success", "", "Indberetningerne blev godkendt med kontering " + $scope.selectedAccount.Description + " - " + $scope.selectedAccount.Number);
           }
       }

       $scope.approveSelectedWithAccountClick = function () {
           if ($scope.selectedAccount == undefined) {
               $scope.errorMessage = "* Du skal vælge en konto";
           } else {
               $modalInstance.close($scope.selectedAccount.Number);
               NotificationService.AutoFadeNotification("success", "", "Indberetningerne blev godkendt med kontering " + $scope.selectedAccount.Description + " - " + $scope.selectedAccount.Number);
           }
       }

   }
]);