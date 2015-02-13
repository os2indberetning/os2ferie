angular.module("application").controller('AddressEditModalInstanceController', ["$scope", "$modalInstance", "addresses", function ($scope, $modalInstance, addresses) {

    $scope.addresses = addresses;

    $scope.closeAddressEditModal = function () {
        $modalInstance.dismiss('Luk');
    };
}]);