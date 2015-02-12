angular.module("application").controller('RouteEditModalController', ["$scope", "$modal", function ($scope, $modal) {

    $scope.items = ['Nr. 1: 123456', 'Nr. 1: 234567', 'Nr. 1: 345678'];

    $scope.openRouteEditModal = function (id) {

        var modalInstance = $modal.open({
            templateUrl: '/App/Settings/RouteEditModal.html',
            controller: 'RouteEditModalInstanceController',
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

angular.module("application").controller('RouteEditModalInstanceController', ["$scope", "$modalInstance", "items", function ($scope, $modalInstance, id) {

    $scope.id = id;

    $scope.closeRouteEditModal = function () {
        $modalInstance.dismiss('Luk');
    };
}]);