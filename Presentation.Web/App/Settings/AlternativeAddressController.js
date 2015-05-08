angular.module("application").controller('AlternativeAddressController', ["$scope", "SmartAdresseSource", "$rootScope", "$timeout", "PersonEmployments", "AddressFormatter", "Address", "NotificationService", "PersonalAddress", function ($scope, SmartAdresseSource, $rootScope, $timeout, PersonEmployments, AddressFormatter, Address, NotificationService, PersonalAddress) {

    $scope.employments = $rootScope.CurrentUser.Employments;
    $scope.homeAddress = "";
    $scope.alternativeHomeAddress = {};
    $scope.alternativeHomeAddress.string = "";

    PersonalAddress.GetRealHomeForUser({ id: $rootScope.CurrentUser.Id }).$promise.then(function (res) {
        $scope.homeAddress = res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town;
    });

    PersonalAddress.GetAlternativeHomeForUser({ id: $rootScope.CurrentUser.Id }).$promise.then(function (res) {
        if (!(res.StreetNumber == undefined)) {
            $scope.alternativeHomeAddress = res;
            $scope.alternativeHomeAddress.string = res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town;
        }
    });
    $scope.Number = Number;
    $scope.toString = toString;
    $scope.replace = String.replace;
    $scope.alternativeWorkAddresses = [];
    $scope.alternativeWorkDistances = [];

    var loadLocalModel = function () {
        angular.forEach($scope.employments, function (empl, key) {
            if (empl.AlternativeWorkAddress != null) {
                var addr = empl.AlternativeWorkAddress;
                $scope.alternativeWorkAddresses[key] = addr.StreetName + " " + addr.StreetNumber + ", " + addr.ZipCode + " " + addr.Town;
            } else if (empl.WorkDistanceOverride != "" && empl.WorkDistanceOverride != null) {
                $scope.alternativeWorkDistances[key] = empl.WorkDistanceOverride;
            }
        });
    }

    loadLocalModel();

    var isAddressSet = function (index) {
        return ($scope.alternativeWorkAddresses[index] != "" && $scope.alternativeWorkAddresses[index] != undefined);
    }

    var isDistanceSet = function (index) {
        return ($scope.alternativeWorkDistances[index] != "" && $scope.alternativeWorkDistances[index] != undefined);
    }

    $scope.saveAlternativeWorkAddress = function (index) {
        // Timeout to allow the address to be written to the model.
        $timeout(function () {
            handleSaveAlternativeWork(index);
        });
    }

    var handleSaveAlternativeWork = function (index) {
        // Both fields empty. Clear.
        if (!isAddressSet(index) && (!isDistanceSet(index) || $scope.alternativeWorkDistances[index] == 0)) {
            $scope.clearWorkClicked(index);
        }
            // Address is set. Save it.
        else if (isAddressSet(index)) {
            var addr = AddressFormatter.fn($scope.alternativeWorkAddresses[index]);
            // No alternative address exists. Post.
            if ($scope.employments[index].AlternativeWorkAddress == null || $scope.employments[index].AlternativeWorkAddress == undefined) {
                Address.post({
                    StreetName: addr.StreetName,
                    StreetNumber: addr.StreetNumber,
                    Town: addr.Town,
                    ZipCode: addr.ZipCode,
                    PersonId: $rootScope.CurrentUser.Id,
                    Description: "Afvigende " + $scope.employments[index].OrgUnit.LongDescription,
                    Longitude: "",
                    Latitude: "",
                    Type: "AlternativeWork"
                }).$promise.then(function (res) {
                    $scope.employments[index].AlternativeWorkAddress = res;
                    $scope.employments[index].AlternativeWorkAddressId = res.Id;
                    loadLocalModel();

                    PersonEmployments.patchEmployment({ id: $scope.employments[index].Id }, { AlternativeWorkAddressId: res.Id }).$promise.then(function () {
                        NotificationService.AutoFadeNotification("success", "", "Afvigende arbejdsadresse oprettet.");
                    });
                });
            }
                // Alternative Address already exists. Patch it.
            else {
                Address.patch({ id: $scope.employments[index].AlternativeWorkAddressId }, {
                    StreetName: addr.StreetName,
                    StreetNumber: addr.StreetNumber,
                    Town: addr.Town,
                    ZipCode: addr.ZipCode,
                    Longitude: "",
                    Latitude: "",
                }).$promise.then(function () {
                    NotificationService.AutoFadeNotification("success", "", "Afvigende arbejdsadresse redigeret.");
                });
            }
        }
            // Address is not set. Distance is. Save that.
        else if (Number($scope.alternativeWorkDistances[index]) >= 0) {
            PersonEmployments.patchEmployment({ id: $scope.employments[index].Id },
            {
                WorkDistanceOverride: $scope.alternativeWorkDistances[index],
                AlternativeWorkAddress: null
            }).$promise.then(function () {
                // Clear local model
                $scope.employments[index].AlternativeWorkAddress = null;
                $scope.employments[index].AlternativeWorkAddressId = null;
                loadLocalModel();
                NotificationService.AutoFadeNotification("success", "", "Afvigende afstand mellem hjem og arbejde gemt.");
            });
        }
    }


    $scope.clearWorkClicked = function (index) {
        PersonEmployments.patchEmployment({ id: $scope.employments[index].Id }, {
            WorkDistanceOverride: 0,
            AlternativeWorkAddress: null,
            AlternativeWorkAddressId: null,
        }).$promise.then(function () {
            $scope.alternativeWorkDistances[index] = 0;
            $scope.alternativeWorkAddresses[index] = "";
            if ($scope.employments[index].AlternativeWorkAddressId != null) {
                Address.delete({ id: $scope.employments[index].AlternativeWorkAddressId });
            }
            $scope.employments[index].AlternativeWorkAddress = null;
            $scope.employments[index].AlternativeWorkAddressId = null;
            NotificationService.AutoFadeNotification("success", "", "Afvigende afstand og adresse slettet.");
        });


    }

    $scope.saveAlternativeHomeAddress = function () {
        $timeout(function () {
            handleSaveAltHome();
        });
    }

    var handleSaveAltHome = function () {
        if ($scope.alternativeHomeAddress.string != undefined && $scope.alternativeHomeAddress.string != null && $scope.alternativeHomeAddress.string != "") {
            var addr = AddressFormatter.fn($scope.alternativeHomeAddress.string);
            if ($scope.alternativeHomeAddress.Id != undefined) {
                PersonalAddress.patch({ id: $scope.alternativeHomeAddress.Id }, {
                    StreetName: addr.StreetName,
                    StreetNumber: addr.StreetNumber,
                    ZipCode: addr.ZipCode,
                    Town: addr.Town,
                    Latitude: "",
                    Longitude: "",
                    Description: "Afvigende hjemmeadresse",
                    Type: "AlternativeHome"
                }).$promise.then(function() {
                    NotificationService.AutoFadeNotification("success", "", "Afvigende hjemmeadresse redigeret.");
                });
            } else {
                PersonalAddress.post({
                    StreetName: addr.StreetName,
                    StreetNumber: addr.StreetNumber,
                    ZipCode: addr.ZipCode,
                    Town: addr.Town,
                    Latitude: "",
                    Longitude: "",
                    PersonId: $rootScope.CurrentUser.Id,
                    Type: "AlternativeHome",
                    Description: "Afvigende hjemmeadresse"
                }).$promise.then(function(res) {
                    $scope.alternativeHomeAddress = res;
                    $scope.alternativeHomeAddress.string = res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town;
                    NotificationService.AutoFadeNotification("success", "", "Afvigende hjemmeadresse oprettet.");
                });
            }
        } else if ($scope.alternativeHomeAddress.string == "" && $scope.alternativeHomeAddress.Id != undefined) {
            $scope.clearHomeClicked();
        }
    }

    $scope.clearHomeClicked = function () {
        $scope.alternativeHomeAddress.string = "";
        if ($scope.alternativeHomeAddress.Id != undefined) {
            PersonalAddress.delete({ id: $scope.alternativeHomeAddress.Id }).$promise.then(function () {
                $scope.alternativeHomeAddress = null;
                NotificationService.AutoFadeNotification("success", "", "Afvigende hjemmeadresse slettet.");
            });
        }
    }


    $scope.SmartAddress = SmartAdresseSource;

}]);