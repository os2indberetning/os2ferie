angular.module("app.drive").controller('RouteDeleteModalInstanceController', [
    "$scope", "Route", "Point", "NotificationService" +
    "", "$modalInstance", "routeId", "personId", "AddressFormatter", function ($scope, Route, Point, NotificationService, $modalInstance, routeId, personId, AddressFormatter) {
  
        $scope.confirmDelete = function () {
            Route.delete({ id: routeId }, function() {
                NotificationService.AutoFadeNotification("success", "", "Rute slettet");
                $modalInstance.close('');
            });
        }

        $scope.cancelDelete = function() {
            $modalInstance.dismiss('');
        }
    
}]);