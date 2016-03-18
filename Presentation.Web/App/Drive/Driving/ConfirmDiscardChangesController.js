angular.module("app.drive").controller("ConfirmDiscardChangesController", [
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