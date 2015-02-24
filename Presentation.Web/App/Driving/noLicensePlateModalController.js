angular.module('application').controller('noLicensePlateModalController', function ($scope, $modalInstance) {
    $scope.ok = function () {
        $modalInstance.close();        
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
});