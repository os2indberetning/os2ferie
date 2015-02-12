angular.module("application").controller('TokenInstanceController', ["$scope", "$modalInstance", "items", function ($scope, $modalInstance, items) {

    $scope.items = items;

    $scope.closeTokenModal = function () {
        $modalInstance.dismiss('Luk');
    };
}]);