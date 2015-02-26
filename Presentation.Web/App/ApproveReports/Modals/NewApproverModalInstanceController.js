angular.module('application').controller('NewApproverModalInstanceController', function ($scope, $modalInstance, persons) {

    $scope.persons = persons;

    $scope.saveNewApprover = function () {
        $modalInstance.close($scope.selected.item);
    };

    $scope.cancelNewApprover = function () {
        $modalInstance.dismiss('cancel');
    };
});