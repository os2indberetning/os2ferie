angular.module("app.drive").controller("AddAdminModalController", [
   "$scope", "chosenPerson", "$modalInstance",
   function ($scope, chosenPerson, $modalInstance) {

   $scope.name = chosenPerson.FullName;

       $scope.confirmAddAdmin = function () {
           /// <summary>
           /// Confirm add new admin
           /// </summary>
           $modalInstance.close(chosenPerson);
       }

       $scope.cancelAddAdmin = function () {
           /// <summary>
           /// Cancel add new admin
           /// </summary>
           $modalInstance.dismiss('cancel');
       }

   }
]);