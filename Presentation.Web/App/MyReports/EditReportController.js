angular.module("application").controller("EditReportController", [
    "$scope", "$modalInstance", function ($scope, $modalInstance) {

        console.log($scope.purpose);
      

     

        $scope.confirmDelete = function () {
            $modalInstance.close($scope.data);
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        }
    }
]);