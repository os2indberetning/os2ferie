angular.module("application").controller("RemoveAdminModalController", [
   "$scope", "Id", "FullName", "$modalInstance",
   function ($scope, Id, FullName, $modalInstance) {

       $scope.name = FullName;

        var resPerson = { Id: Id, FullName: FullName };

       $scope.confirmRemoveAdmin = function () {
           $modalInstance.close(resPerson);
       }

       $scope.cancelRemoveAdmin = function () {
           $modalInstance.dismiss('cancel');
       }

   }
]);