angular.module("application").controller("ConfirmDeleteReportController", [
    "$scope", "$modalInstance", "itemId", function ($scope, $modalInstance, itemId) {

        $scope.itemId = itemId;

        $scope.confirmDelete = function() {
            $modalInstance.close($scope.itemId);
        }

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        }
    }
]);