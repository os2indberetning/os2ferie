angular.module("app.common").controller("ServiceErrorController", [
   "$scope", "$modalInstance", "errorMsg",
   function ($scope, $modalInstance, errorMsg) {

       $scope.errorMsg = errorMsg;

       $scope.close = function () {
           $modalInstance.close();
       }

   }
]);