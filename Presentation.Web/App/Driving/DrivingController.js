angular.module("application").controller("DrivingController", [
    "$scope", function($scope) {

        $scope.DriveReport = {};

        $scope.DriveReport.Addresses = [];

        $scope.DriveReport.Addresses.push({ Name: "", Save: false });
        $scope.DriveReport.Addresses.push({ Name: "", Save: false });


        console.log($scope.DriveReport.Addresses);

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

        $scope.AddDestination = function() {
            $scope.DriveReport.Addresses.push({ Name: "", Save: false });
        }

        $scope.Remove = function (array, index) {
            array.splice(index, 1);
        }
    }
]);