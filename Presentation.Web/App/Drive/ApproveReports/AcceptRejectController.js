angular.module("application").controller("AcceptRejectController", [
    "$scope", "itemId", "$modalInstance", function ($scope, itemId, $modalInstance) {


        $scope.itemId = itemId;

        $scope.noClicked = function() {
            $modalInstance.dismiss('cancel');
        }

        $scope.yesClicked = function() {
            $modalInstance.close($scope.itemId);
        }

    }
]);