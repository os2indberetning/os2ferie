﻿angular.module('app.core').controller('ConfirmDeleteApproverModalInstanceController',
    ["$scope", "$modalInstance", "persons", "orgUnits", "leader", "Substitute", "Person", "NotificationService", "substituteId",
        function ($scope, $modalInstance, persons, orgUnits, leader, Substitute, Person, NotificationService, substituteId) {

        $scope.loadingPromise = null;

        $scope.persons = persons;
        $scope.orgUnits = orgUnits;

        $scope.substitute = Substitute.get({ id: substituteId }, function (data) {

            $scope.substitute = data.value[0]; // This is bad, but can't change the service
            $scope.sub = $scope.substitute.Sub;
            $scope.person = $scope.substitute.Person;
            console.log($scope.substitute);
            $scope.substituteFromDate = new Date($scope.substitute.StartDateTimestamp * 1000).toLocaleDateString();
            if ($scope.substitute.EndDateTimestamp == 9999999999) {
                $scope.substituteToDate = "På ubestemt tid";
            } else {
                $scope.substituteToDate = new Date($scope.substitute.EndDateTimestamp * 1000).toLocaleDateString();
            }
        });

        $scope.orgUnitSelected = function (id) {
            console.log(id);
        }

        $scope.deleteSubstitute = function () {

            var sub = new Substitute();

            $scope.showSpinner = true;

            $scope.loadingPromise = sub.$delete({ id: $scope.substitute.Id }, function (data) {
                NotificationService.AutoFadeNotification("success", "", "Personlig godkender er blevet slettet");
                $modalInstance.close();
            }, function () {
                NotificationService.AutoFadeNotification("danger", "", "Kunne ikke slette personlig godkender");
            });
        };

        $scope.cancelSubstitute = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);