angular.module("application").controller("DrivingController", [
    "$scope", function($scope) {

        $scope.DriveReport = {};

        $scope.DriveReport.Date = new Date();

        $scope.DateOptions = {
            start: "month"
        }

  
        $scope.DrivenKilometers = 33;
        $scope.TransportAllowance = 33;
        $scope.RemainingKilometers = 0;
        $scope.PayoutAmount = 123;

        $scope.print = function() {
            console.log($scope.DriveReport);
        }
    }
]);