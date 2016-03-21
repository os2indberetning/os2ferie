angular.module("app.drive").controller('TokenInstanceController', ["$scope", "NotificationService", "$modalInstance", "Token", "personId", "$modal", function ($scope, NotificationService, $modalInstance, Token, personId, $modal) {




}]);


angular.module('app.drive').controller('AppLoginModalController', ["$scope","$modalInstance", function ($scope, $modalInstance) {
    
    $scope.confirmDelete = function () {
        $modalInstance.close();
    };

    $scope.cancelDelete = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.createAppPassword = function () {
        $modalInstance.close($scope.password);
    }
}]);