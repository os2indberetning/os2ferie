angular.module("application").controller("ConfirmDiscardChangesController", [
   "$scope", "$modalInstance",
   function ($scope, $modalInstance) {

       $scope.confirmDiscardChanges = function () {
           $modalInstance.close();
       }

       $scope.cancel = function () {
           $modalInstance.dismiss('cancel');
       }

   }
]);