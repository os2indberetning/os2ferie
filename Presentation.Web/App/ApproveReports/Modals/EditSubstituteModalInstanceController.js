angular.module('application').controller('EditSubstituteModalInstanceController',
    ["$scope", "$modalInstance", "persons", "OrgUnit", "leader", "Substitute", "Person", "NotificationService", "substituteId", function ($scope, $modalInstance, persons, OrgUnit, leader, Substitute, Person, NotificationService, substituteId) {


        $scope.persons = persons;

        $scope.person = [];

        $scope.substitute = Substitute.get({ id: substituteId }, function (data) {
            if (data.value[0].EndDateTimestamp == 9999999999) {
                $scope.infinitePeriod = true;
            }

            OrgUnit.getWhereUserIsLeader({ id: data.value[0].PersonId }).$promise.then(function(res) {
                $scope.orgUnits = res;
            });

            $scope.substitute = data.value[0]; // This is bad, but can't change the service


            $scope.person[0] = $scope.substitute.Sub;
            $scope.substituteFromDate = new Date($scope.substitute.StartDateTimestamp * 1000);
            $scope.substituteToDate = new Date($scope.substitute.EndDateTimestamp * 1000);
        });

        $scope.saveNewSubstitute = function () {
            if ($scope.person == undefined) {
                NotificationService.AutoFadeNotification("danger", "", "Du skal vælge en person");
                return;
            }

            var sub = new Substitute({
                StartDateTimestamp: Math.floor($scope.substituteFromDate.getTime() / 1000),
                EndDateTimestamp: Math.floor($scope.substituteToDate.getTime() / 1000),
                SubId: $scope.person[0].Id,
                OrgUnitId: $scope.orgUnit.Id
            });

            if ($scope.infinitePeriod) {
                sub.EndDateTimestamp = 9999999999;
            }

            sub.$patch({ id: $scope.substitute.Id }, function (data) {
                NotificationService.AutoFadeNotification("success", "", "Stedfortræder blev gemt");
                $modalInstance.close();
            }, function () {
                NotificationService.AutoFadeNotification("danger", "", "Kunne ikke gemme stedfortræder (Du kan ikke oprette 2 stedfortrædere for samme organisation i samme periode)");
            });
        };

        $scope.cancelNewSubstitute = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);