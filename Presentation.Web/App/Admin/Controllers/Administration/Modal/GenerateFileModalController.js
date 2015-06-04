angular.module("application").controller("GenerateFileModalController", [
   "$scope", "$modalInstance", function ($scope, $modalInstance) {

 
       $scope.confirmGenerateFile = function () {
           /// <summary>
           /// Confirm Generate KMD file
           /// </summary>
           $modalInstance.close();
       }

       $scope.cancelGenerateFile = function () {
           /// <summary>
           /// Cancel generate KMD file.
           /// </summary>
           $modalInstance.dismiss('cancel');
       }

   }
]);