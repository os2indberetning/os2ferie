angular.module("application").controller("AcceptWithAccountController", [
   "$scope", "$modalInstance", "itemId", "BankAccount", "NotificationService", function ($scope, $modalInstance, itemId, BankAccount, NotificationService) {

       $scope.itemId = itemId;

       $scope.result = {};

       BankAccount.get().$promise.then(function (res) {
           $scope.accounts = res.value;
       });

       $scope.noClicked = function () {
           $modalInstance.dismiss('cancel');
           NotificationService.AutoFadeNotification("warning", "Annuller", "Godkendelsen af indberetningen blev annulleret.");
       }

       $scope.yesClicked = function () {
           if ($scope.selectedAccount == undefined) {
               $scope.errorMessage = "* Du skal vælge en konto";
           } else {
               $scope.result.AccountNumber = $scope.selectedAccount.Number;
               $scope.result.Id = itemId;
               $modalInstance.close($scope.result);
               NotificationService.AutoFadeNotification("success", "Godkendt", "Indberetningen blev godkendt med kontering " + $scope.selectedAccount.Description + " - " + $scope.selectedAccount.Number);
           }
       }

   }
]);