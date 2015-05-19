angular.module('application').controller('AdminNewSubstituteModalInstanceController',
    ["$scope", "$modalInstance", "OrgUnit", "leader", "Substitute", "Person", "NotificationService", "$timeout", "persons",
        function ($scope, $modalInstance, OrgUnit, leader, Substitute, Person, NotificationService, $timeout, persons) {

            $scope.persons = persons;

            Person.GetLeaders().$promise.then(function (res) {
                $scope.leaders = res;
            });

            $scope.substituteFromDate = new Date();
            $scope.substituteToDate = new Date();
            $scope.orgUnitsDisabled = true;
            $scope.orgUnits = [];



            $scope.personForOptions = {
                select: function (item) {
                    $scope.orgUnitsDisabled = true;
                    $scope.orgUnit = undefined;
                    $timeout(function () {
                        OrgUnit.getWhereUserIsLeader({ id: $scope.personFor[0].Id }, function (res) {
                            $scope.orgUnits = res;
                            if ($scope.orgUnits.length > 0) {
                                $scope.orgUnit = $scope.orgUnits[0];
                                $scope.orgUnitsDisabled = false;
                            }
                        });
                    }, 0);

                }
            }

            $scope.saveNewSubstitute = function () {
                if ($scope.person == undefined) {
                    NotificationService.AutoFadeNotification("danger", "", "Du skal vælge en stedfortræder");
                    return;
                }

                if ($scope.personFor == undefined) {
                    NotificationService.AutoFadeNotification("danger", "", "Du skal vælge en person der skal stedfortrædes for");
                    return;
                }

                if ($scope.orgUnit == undefined) {
                    NotificationService.AutoFadeNotification("danger", "", "Du skal vælge en organisationsenhed.");
                    return;
                }


                var sub = new Substitute({
                    StartDateTimestamp: Math.floor($scope.substituteFromDate.getTime() / 1000),
                    EndDateTimestamp: Math.floor($scope.substituteToDate.getTime() / 1000),
                    LeaderId: $scope.personFor[0].Id,
                    SubId: $scope.person[0].Id,
                    OrgUnitId: $scope.orgUnit.Id,
                    PersonId: $scope.personFor[0].Id
                });

                if ($scope.infinitePeriod) {
                    sub.EndDateTimestamp = 9999999999;
                }

                sub.$post(function (data) {
                    NotificationService.AutoFadeNotification("success", "", "Stedfortræder blev oprettet");
                    $modalInstance.close();
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "", "Kunne ikke oprette stedfortræder");
                });
            };

            $scope.cancelNewSubstitute = function () {
                $modalInstance.dismiss('cancel');
            };
        }]);