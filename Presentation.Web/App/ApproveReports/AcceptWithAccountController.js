angular.module("application").controller("AcceptWithAccountController", [
   "$scope", "$modalInstance", "itemId", "BankAccount", function ($scope, $modalInstance, itemId, BankAccount) {

       $scope.itemId = itemId;

       $scope.result = {};

       BankAccount.get().$promise.then(function (res) {
           $scope.accounts = res.value;
       });

       $scope.noClicked = function () {
           $modalInstance.dismiss('cancel');
       }

       $scope.yesClicked = function () {
           if ($scope.selectedAccount == undefined) {
               $scope.errorMessage = "* Du skal vælge en konto";
           } else {
               $scope.result.AccountNumber = $scope.selectedAccount.Number;
               $scope.result.Id = itemId;
               $modalInstance.close($scope.result);
           }
       }

   }
]);