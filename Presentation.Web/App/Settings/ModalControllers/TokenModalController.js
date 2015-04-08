angular.module("application").controller('TokenInstanceController', ["$scope", "NotificationService", "$modalInstance", "Token", "personId", "$modal", function ($scope, NotificationService, $modalInstance, Token, personId, $modal) {




}]);

angular.module('application').controller('confirmDeleteToken', function ($scope, $modalInstance, token) {
    
    $scope.confirmDelete = function () {
        $modalInstance.close(token);
    };

    $scope.cancelDelete = function () {
        $modalInstance.dismiss('cancel');
    };
});