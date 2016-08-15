angular.module('app.core').controller('EditSubstituteModalInstanceController',
    ["$scope", "$modalInstance", "persons", "OrgUnit", "leader", "Substitute", "Person", "NotificationService", "substituteId", "ReportType",
    function ($scope, $modalInstance, persons, OrgUnit, leader, Substitute, Person, NotificationService, substituteId, ReportType) {

        $scope.container = {};

        $scope.persons = persons;

        $scope.loadingPromise = null;

        $scope.person = [];

        $scope.autoCompleteOptions = {
            select: function (e) {
                $scope.person[0] = this.dataItem(e.item.index());
            }
        }

        $scope.substitute = Substitute.get({ id: substituteId }, function (data) {
            if (data.value[0].EndDateTimestamp == 9999999999) {
                $scope.infinitePeriod = true;
            }

            OrgUnit.getWhereUserIsLeader({ id: data.value[0].PersonId }).$promise.then(function(res) {
                $scope.orgUnits = res;
            });

            $scope.substitute = data.value[0]; // This is bad, but can't change the service
            $scope.orgUnit = $scope.substitute.OrgUnit;
            $scope.person[0] = $scope.substitute.Sub;
            $scope.substituteFromDate = new Date($scope.substitute.StartDateTimestamp * 1000);
            $scope.substituteToDate = new Date($scope.substitute.EndDateTimestamp * 1000);
            $scope.container.autoComplete.value($scope.substitute.Sub.FullName);
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
                OrgUnitId: $scope.orgUnit.Id,
                CreatedById: leader.Id,
                Type: ReportType === 0 ? "Drive" : "Vacation"
            });

            if ($scope.infinitePeriod) {
                sub.EndDateTimestamp = 9999999999;
            }

            $scope.showSpinner = true;

            $scope.loadingPromise = sub.$patch({ id: $scope.substitute.Id }, function (data) {
                NotificationService.AutoFadeNotification("success", "", "Stedfortræder blev gemt");
                $modalInstance.close();
            }, function () {
                NotificationService.AutoFadeNotification("danger", "", "Kunne ikke gemme stedfortræder (Du kan ikke oprette 2 stedfortrædere for samme organisation i samme periode)");
                $scope.showSpinner = false;
            });
        };

        $scope.cancelNewSubstitute = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);
