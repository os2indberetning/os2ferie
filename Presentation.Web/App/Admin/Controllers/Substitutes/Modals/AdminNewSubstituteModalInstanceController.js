angular.module('application').controller('AdminNewSubstituteModalInstanceController',
    ["$scope", "$modalInstance", "OrgUnit", "leader", "Substitute", "Person", "NotificationService", "$timeout", "persons", "Autocomplete",
        function ($scope, $modalInstance, OrgUnit, leader, Substitute, Person, NotificationService, $timeout, persons, Autocomplete) {

            $scope.persons = persons;

            var infinitePeriod = 9999999999;

            $scope.autoCompleteOptions = {
                filter: "contains"
            };


            $scope.leaders = Autocomplete.leaders();


            $scope.substituteFromDate = new Date();
            $scope.substituteToDate = new Date();
            $scope.orgUnitsDisabled = true;
            $scope.orgUnits = [];



            $scope.personForOptions = {
                filter: "contains",
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
                /// <summary>
                /// Post new substitute to backend if fields are filled correctly.
                /// </summary>
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
                    sub.EndDateTimestamp = infinitePeriod;
                }

                $scope.showSpinner = true;

                sub.$post(function (data) {
                    NotificationService.AutoFadeNotification("success", "", "Stedfortræder blev oprettet");
                    $modalInstance.close();
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "", "Kunne ikke oprette stedfortræder (Du kan ikke oprette 2 stedfortrædere for samme organisation i samme periode)");
                });
            };

            $scope.cancelNewSubstitute = function () {
                $modalInstance.dismiss('cancel');
            };
        }]);