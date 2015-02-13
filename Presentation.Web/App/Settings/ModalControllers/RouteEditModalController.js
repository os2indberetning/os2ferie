angular.module("application").controller('RouteEditModalInstanceController', ["$scope", "NotificationService", "$modalInstance", "routes", function ($scope, NotificationService, $modalInstance, routes) {

    $scope.saveUpdatedRoute = function () {
        NotificationService.AutoFadeNotification("danger", "Success", "Jeg er ikke implementeret");
    }

    $scope.closeRouteEditModal = function () {
        $modalInstance.dismiss('Luk');
    };
}]);