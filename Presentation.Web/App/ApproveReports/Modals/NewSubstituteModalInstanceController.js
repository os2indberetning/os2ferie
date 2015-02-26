angular.module('application').controller('NewSubstituteModalInstanceController', function ($scope, $modalInstance, persons) {

    $scope.persons = persons;

    $scope.saveNewSubstitute = function () {
        $modalInstance.close($scope.selected.item);
    };

    $scope.cancelNewSubstitute = function () {
        $modalInstance.dismiss('cancel');
    };
});