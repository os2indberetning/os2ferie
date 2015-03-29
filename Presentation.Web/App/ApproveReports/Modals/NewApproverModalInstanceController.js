angular.module('application').controller('NewApproverModalInstanceController', ["$scope", "$modalInstance", "persons", "orgUnits", "leader", function ($scope, $modalInstance, persons, orgUnits, leader) {
    $scope.persons = persons;
    $scope.approverFromDate = new Date();
    $scope.approverToDate = new Date();
    $scope.orgUnits = orgUnits;
    $scope.orgUnit = $scope.orgUnits[0];



    $scope.saveNewApprover = function () {
        //$modalInstance.close($scope.selected.item);

        console.log($scope.approver);
        console.log($scope.target);

    };

    $scope.cancelNewApprover = function () {
        $modalInstance.dismiss('cancel');
    };
}]);