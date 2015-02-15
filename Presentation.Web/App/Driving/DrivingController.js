angular.module("application").controller("DrivingController", [
    "$scope", "SmartAdresseSource", "DriveReport", function($scope, SmartAdresseSource, DriveReport) {

        $scope.DriveReport = new DriveReport();

        $scope.DriveReport.Addresses = [];

        $scope.DriveReport.Addresses.push({ Name: "", Save: false });
        $scope.DriveReport.Addresses.push({ Name: "", Save: false });

        $scope.SmartAddress = SmartAdresseSource;

        console.log($scope.DriveReport.Addresses);

        $scope.DriveReport.Date = new Date();

        $scope.DateOptions = {
            start: "month"
        };

  
        $scope.DrivenKilometers = 33;
        $scope.TransportAllowance = 33;
        $scope.RemainingKilometers = 0;
        $scope.PayoutAmount = 123;

        $scope.Save = function() {
            console.log($scope.DriveReport);
            $scope.DriveReport.$save();
        };

        $scope.AddDestination = function() {
            $scope.DriveReport.Addresses.push({ Name: "", Save: false });
        };

        $scope.Remove = function (array, index) {
            array.splice(index, 1);
        };
    }
]);