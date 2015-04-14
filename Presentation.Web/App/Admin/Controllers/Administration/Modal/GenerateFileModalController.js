angular.module("application").controller("GenerateFileModalController", [
   "$scope", "$modalInstance", function ($scope, $modalInstance) {

 
       $scope.confirmGenerateFile = function () {
           $modalInstance.close();
       }

       $scope.cancelGenerateFile = function () {
           $modalInstance.dismiss('cancel');
       }

   }
]);