angular.module('application').controller('EditApproverModalInstanceController',
    ["$scope", "$modalInstance", "persons", "orgUnits", "leader", "Substitute", "Person", "NotificationService", "substituteId", function ($scope, $modalInstance, persons, orgUnits, leader, Substitute, Person, NotificationService, substituteId) {
        $scope.persons = persons;
        $scope.approverFromDate = new Date();
        $scope.approverToDate = new Date();
        $scope.orgUnits = orgUnits;
        $scope.orgUnit = $scope.orgUnits[0];
        $scope.approver = [];
        $scope.target = [];

        $scope.substitute = Substitute.get({ id: substituteId }, function (data) {

            $scope.substitute = data.value[0]; // Should change the service

            $scope.target[0] = $scope.substitute.Persons[0];
            $scope.approver[0] = $scope.substitute.Sub;

            $scope.substituteFromDate = new Date($scope.substitute.StartDateTimestamp * 1000);
            $scope.substituteToDate = new Date($scope.substitute.EndDateTimestamp * 1000);
            $scope.orgUnit = $.grep($scope.orgUnits, function (e) { return e.Id == $scope.substitute.OrgUnitId; })[0];

        });

        $scope.saveNewApprover = function () {
            if ($scope.approver == undefined) {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Du skal vælge en godkender");
                return;
            }

            if ($scope.target == undefined) {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Du skal vælge en ansat");
                return;
            }
            
            var sub = new Substitute({
                StartDateTimestamp: Math.floor($scope.approverFromDate.getTime() / 1000),
                EndDateTimestamp: Math.floor($scope.approverToDate.getTime() / 1000),
                LeaderId: leader.Id,
                SubId: $scope.approver[0].Id,
                OrgUnitId: $scope.orgUnit.Id,
                Persons: [$scope.target[0].Id]
            });

            sub.$patch({ id: substituteId }, function (data) {
                NotificationService.AutoFadeNotification("success", "Success", "Stedfortræder blev oprettet");
                $modalInstance.close();
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke oprette stedfortræder");
            });
        };

        $scope.cancelNewApprover = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);