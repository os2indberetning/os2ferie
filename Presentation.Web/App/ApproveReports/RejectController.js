angular.module("application").controller("RejectController", [
   "$scope", "$modalInstance", "itemId", function ($scope, $modalInstance, itemId) {

       $scope.itemId = itemId;

       $scope.result = {};


       $scope.noClicked = function () {
           $modalInstance.dismiss('cancel');
       }

       $scope.yesClicked = function () {
           if ($scope.comment == undefined) {
               $scope.result.Comment = "";
           } else {
               $scope.result.Comment = $scope.comment;
           }
           $scope.result.Id = itemId;
           $modalInstance.close($scope.result);
       }

   }
]);