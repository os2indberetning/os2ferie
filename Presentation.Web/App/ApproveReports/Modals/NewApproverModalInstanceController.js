angular.module('application').controller('NewApproverModalInstanceController',
    ["$scope", "$modalInstance", "persons", "orgUnits", "leader", "Substitute", "Person", "NotificationService", function ($scope, $modalInstance, persons, orgUnits, leader, Substitute, Person, NotificationService) {

        $scope.persons = persons;
        $scope.approverFromDate = new Date();
        $scope.approverToDate = new Date();
        $scope.orgUnits = orgUnits;
        $scope.orgUnit = $scope.orgUnits[0];

        $scope.autoCompleteOptions = {
            filter: "contains"
        };

        $scope.personsWithoutLeader = $scope.persons.slice(0); // Clone array;

        // Remove leader from array
        angular.forEach($scope.persons, function (value, key) {
            if (value.Id == leader.Id) {
                $scope.personsWithoutLeader.splice(key, 1);
            }
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
                LeaderId: leader.Id,
                SubId: $scope.approver[0].Id,
                OrgUnitId: $scope.orgUnit.Id,
                PersonId: $scope.target[0].Id
            });

            if ($scope.infinitePeriod) {
                sub.EndDateTimestamp = 9999999999;
            }

            sub.$post(function (data) {
                NotificationService.AutoFadeNotification("success", "", "Godkender blev oprettet");
                $modalInstance.close();
            }, function () {
                NotificationService.AutoFadeNotification("danger", "", "Kunne ikke oprette godkender (Du kan ikke oprette 2 godkendere for samme person i samme periode)");
            });
        };

        $scope.cancelNewApprover = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);