angular.module("application").controller("MainMenuController", [
   "$scope", "Person",function ($scope, Person) {

        Person.GetCurrentUser().$promise.then(function(res) {
            $scope.showAdministration = res.IsAdmin;
            $scope.showApproveReports = res.IsLeader;
        });

    }
]);