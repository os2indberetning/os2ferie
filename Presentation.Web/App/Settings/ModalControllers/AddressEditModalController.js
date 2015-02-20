angular.module("application").controller('AddressEditModalController', ["$scope", "$modal", function ($scope, $modal) {

    $scope.items = ['Nr. 1: 123456', 'Nr. 1: 234567', 'Nr. 1: 345678'];

    $scope.openAddressEditModal = function (id) {

        var modalInstance = $modal.open({
            templateUrl: '/App/Settings/AddressEditModal.html',
            controller: 'AddressEditModalInstanceController',
            resolve: {
                items: function () {
                    return id;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        });
    };
}]);

angular.module("application").controller('AddressEditModalInstanceController', ["$scope", "$modalInstance", "items", function ($scope, $modalInstance, id) {

    $scope.id = id;

    $scope.closeAddressEditModal = function () {
        $modalInstance.dismiss('Luk');
    };
}]);