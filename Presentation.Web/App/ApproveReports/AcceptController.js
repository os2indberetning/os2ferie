angular.module("application").controller("AcceptController", [
   "$scope", "$modalInstance", "itemId", function ($scope, $modalInstance, itemId) {

        $scope.itemId = itemId;

       $scope.noClicked = function () {
           $modalInstance.dismiss('cancel');
       }

       $scope.yesClicked = function () {
           $modalInstance.close($scope.itemId);
       }

   }
]);