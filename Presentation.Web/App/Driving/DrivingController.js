angular.module("application").controller("DrivingController", [
    "$scope", function($scope) {
        $scope.date = {
            start: "month",
            value: new Date()
        }
    }
]);