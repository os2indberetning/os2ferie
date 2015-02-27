angular.module('application').controller('NewSubstituteModalInstanceController',
    ["$scope", "$modalInstance", "persons", "orgUnits", "leader", "Substitute", "Person", "NotificationService", function ($scope, $modalInstance, persons, orgUnits, leader, Substitute, Person, NotificationService) {
        $scope.persons = persons;
        $scope.substituteFromDate = new Date();
        $scope.substituteToDate = new Date();
        $scope.orgUnits = orgUnits;
        $scope.orgUnit = $scope.orgUnits[0];
        
        $scope.orgUnitSelected = function (id) {
            console.log(id);
        }

        $scope.saveNewSubstitute = function () {
            if ($scope.person == undefined) {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Du skal vælge en person");
                return;
            }

            var sub = new Substitute({
                StartDateTimestamp: 1,
                EndDateTimestamp: 1,
                LeaderId: 1,
                SubId: $scope.person[0].Id,
                OrgUnitId: $scope.orgUnit.Id,
                Persons: [leader]
            });

            sub.$post(function (data) {
                NotificationService.AutoFadeNotification("success", "Success", "Stedfortræder blev oprettet");
                $modalInstance.close();
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke oprette stedfortræder");
            });
        };

        $scope.cancelNewSubstitute = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);