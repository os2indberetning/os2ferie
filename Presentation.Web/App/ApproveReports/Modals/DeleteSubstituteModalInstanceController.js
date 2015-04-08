﻿angular.module('application').controller('ConfirmDeleteSubstituteModalInstanceController',
    ["$scope", "$modalInstance", "persons", "orgUnits", "leader", "Substitute", "Person", "NotificationService", "substituteId", function ($scope, $modalInstance, persons, orgUnits, leader, Substitute, Person, NotificationService, substituteId) {

        $scope.persons = persons;
        $scope.orgUnits = orgUnits;

        $scope.substitute = Substitute.get({ id: substituteId }, function (data) {

            $scope.substitute = data.value[0]; // This is bad, but can't change the service
            $scope.person = $scope.substitute.Sub;
            console.log($scope.substitute);
            $scope.substituteFromDate = new Date($scope.substitute.StartDateTimestamp * 1000).toLocaleDateString();
            if ($scope.substitute.EndDateTimestamp == 9999999999) {
                $scope.substituteToDate = "tidsubegrænset";
            } else {
                $scope.substituteToDate = new Date($scope.substitute.EndDateTimestamp * 1000).toLocaleDateString();
            }
        });

        
        $scope.orgUnitSelected = function (id) {
            console.log(id);
        }

        $scope.deleteSubstitute = function () {

            var sub = new Substitute();

            sub.$delete({ id: $scope.substitute.Id }, function (data) {
                NotificationService.AutoFadeNotification("success", "Success", "Stedfortræderen er blev slettet gemt");
                $modalInstance.close();
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke slette stedfortræder");
            });
        };

        $scope.cancelSubstitute = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);