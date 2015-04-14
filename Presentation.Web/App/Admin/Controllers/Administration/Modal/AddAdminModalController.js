angular.module("application").controller("AddAdminModalController", [
   "$scope", "chosenPerson", "$modalInstance",
   function ($scope, chosenPerson, $modalInstance) {

   $scope.name = chosenPerson.FullName;

       $scope.confirmAddAdmin = function () {
           $modalInstance.close(chosenPerson);
       }

       $scope.cancelAddAdmin = function () {
           $modalInstance.dismiss('cancel');
       }

   }
]);