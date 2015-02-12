angular.module("application").controller('RouteEditModalController', ["$scope", "$modal", function ($scope, $modal) {

    $scope.items = ['Nr. 1: 123456', 'Nr. 1: 234567', 'Nr. 1: 345678'];

    $scope.openRouteEditModal = function (size) {

        var modalInstance = $modal.open({
            templateUrl: '/App/Settings/tokenModal.html',
            controller: 'TokenInstanceController',
            size: size,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        });
    };
}]);

angular.module("application").controller('RouteEditModalInstanceController', ["$scope", "$modalInstance", "items", function ($scope, $modalInstance, items) {

    $scope.items = items;
    $scope.selected = {
        item: $scope.items[0]
    };

    $scope.closeRouteEditModal = function () {
        $modalInstance.dismiss('Luk');
    };
}]);