angular.module('application').controller('EditApproverModalInstanceController',
    ["$scope", "$modalInstance", "persons", "orgUnits", "leader", "Substitute", "Person", "NotificationService", "substituteId",
        function ($scope, $modalInstance, persons, orgUnits, leader, Substitute, Person, NotificationService, substituteId) {

            $scope.persons = persons;
            $scope.orgUnits = orgUnits;
            $scope.orgUnit = $scope.orgUnits[0];

            $scope.personsWithoutLeader = $scope.persons.slice(0); // Clone array;

            // Remove leader from array
            angular.forEach($scope.persons, function (value, key) {
                if (value.Id == leader.Id) {
                    $scope.personsWithoutLeader.splice(key, 1);
                }
            });

            $scope.substitute = Substitute.get({ id: substituteId }, function (data) {

                if (data.value[0].EndDateTimestamp == 9999999999) {
                    $scope.infinitePeriod = true;
                }


                $scope.substitute = data.value[0]; // Should change the service

                $scope.target = $scope.substitute.Person;
                $scope.approver = $scope.substitute.Sub;

                $scope.approverFromDate = new Date($scope.substitute.StartDateTimestamp * 1000);
                $scope.approverToDate = new Date($scope.substitute.EndDateTimestamp * 1000);
                $scope.orgUnit = $.grep($scope.orgUnits, function (e) { return e.Id == $scope.substitute.OrgUnitId; })[0];
            });

            $scope.saveNewApprover = function () {
                if ($scope.approver == undefined) {
                    NotificationService.AutoFadeNotification("danger", "", "Du skal vælge en godkender");
                    return;
                }

                if ($scope.target == undefined) {
                    NotificationService.AutoFadeNotification("danger", "", "Du skal vælge en ansat");
                    return;
                }

                var sub = new Substitute({
                    StartDateTimestamp: Math.floor($scope.approverFromDate.getTime() / 1000),
                    EndDateTimestamp: Math.floor($scope.approverToDate.getTime() / 1000),
                    SubId: $scope.approver.Id,
                    OrgUnitId: $scope.orgUnit.Id,
                    PersonId: $scope.target.Id
                });

                if ($scope.infinitePeriod) {
                    sub.EndDateTimestamp = 9999999999;
                }

                sub.$patch({ id: substituteId }, function (data) {
                    NotificationService.AutoFadeNotification("success", "", "Godkender blev redigeret");
                    $modalInstance.close();
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "", "Kunne ikke oprette godkender");
                });
            };

            $scope.cancelNewApprover = function () {
                $modalInstance.dismiss('cancel');
            };
        }]);