angular.module("application").controller('AlternativeAddressController', ["$scope", "SmartAdresseSource", "$rootScope", "$timeout", "PersonEmployments", "AddressFormatter", "Address", "NotificationService", "PersonalAddress", function ($scope, SmartAdresseSource, $rootScope, $timeout, PersonEmployments, AddressFormatter, Address, NotificationService, PersonalAddress) {

    $scope.employments = $rootScope.CurrentUser.Employments;
    $scope.homeAddress = "";
    $scope.alternativeHomeAddress = {};
    $scope.alternativeHomeAddress.string = "";

    var homeAddressIsDirty = false;
    var workAddressDirty = [];
    var workDistanceDirty = [];

    PersonalAddress.GetRealHomeForUser({ id: $rootScope.CurrentUser.Id }).$promise.then(function (res) {
        $scope.homeAddress = res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town;
    });

    $scope.AlternativeWorkAddressHelpText = $rootScope.HelpTexts.AlternativeWorkAddressHelpText.text;
    $scope.AlternativeWorkDistanceHelpText = $rootScope.HelpTexts.AlternativeWorkDistanceHelpText.text;

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
        /// <summary>
        /// Fills local model variables with data.
        /// </summary>
        angular.forEach($scope.employments, function (empl, key) {
            if (empl.AlternativeWorkAddress != null) {
                var addr = empl.AlternativeWorkAddress;
                $scope.alternativeWorkAddresses[key] = addr.StreetName + " " + addr.StreetNumber + ", " + addr.ZipCode + " " + addr.Town;
            } if (empl.WorkDistanceOverride != "" && empl.WorkDistanceOverride != null) {
                $scope.alternativeWorkDistances[key] = empl.WorkDistanceOverride;
            }
        });
    }

    loadLocalModel();

      $scope.alternativeWorkDistanceChanged = function ($index) {
        /// <summary>
        /// Sets the address to be dirty when changed. This is used when prompting the user when leaving a page with unsaved changes.
        /// </summary>
        /// <param name="$index"></param>
        workDistanceDirty[$index] = true;
    }

    $scope.alternativeWorkAddressChanged = function ($index) {
        /// <summary>
        /// Sets the distance to be dirty when changed. This is used when prompting the user when leaving a page with unsaved changes.
        /// </summary>
        /// <param name="$index"></param>
        workAddressDirty[$index] = true;
    }

    var isAddressSet = function (index) {
        return ($scope.alternativeWorkAddresses[index] != "" && $scope.alternativeWorkAddresses[index] != undefined);
    }

    var isDistanceSet = function (index) {
        return ($scope.alternativeWorkDistances[index] != "" && $scope.alternativeWorkDistances[index] != undefined && $scope.alternativeWorkDistances > 0);
    }

    $scope.saveAlternativeWorkAddress = function (index) {
        // Timeout to allow the address to be written to the model.
        $timeout(function () {
            handleSaveAlternativeWork(index);
        });
    }

    var handleSavingAlternativeAddress = function(index){
        // Save alternative address
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
                workAddressDirty[index] = false;
                $scope.employments[index].AlternativeWorkAddress = res;
                $scope.employments[index].AlternativeWorkAddressId = res.Id;

                PersonEmployments.patchEmployment({ id: $scope.employments[index].Id }, { AlternativeWorkAddressId: res.Id }).$promise.then(function () {
                    NotificationService.AutoFadeNotification("success", "", "Afvigende arbejdsadresse oprettet.");
                    $rootScope.$emit('PersonalAddressesChanged');
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
            }).$promise.then(function (res) {
                $scope.employments[index].AlternativeWorkAddress = res;
                $scope.employments[index].AlternativeWorkAddressId = res.Id;
                workAddressDirty[index] = false;
                NotificationService.AutoFadeNotification("success", "", "Afvigende arbejdsadresse redigeret.");
                $rootScope.$emit('PersonalAddressesChanged');
            });
        }
    }

    var handleSavingAlternativeDistance = function (index) {
        if($scope.alternativeWorkDistances[index] == ""){
            $scope.alternativeWorkDistances[index] = 0;
        }
        PersonEmployments.patchEmployment({ id: $scope.employments[index].Id },
           {
               WorkDistanceOverride: $scope.alternativeWorkDistances[index],
           }).$promise.then(function () {

               workDistanceDirty[index] = false;
               $scope.employments[index].WorkDistanceOverride = $scope.alternativeWorkDistances[index];
               NotificationService.AutoFadeNotification("success", "", "Afvigende afstand mellem hjem og arbejde gemt.");
           });
    }

    var handleSaveAlternativeWork = function (index) {
        /// <summary>
        /// Handles saving alternative work address.
        /// </summary>
        /// <param name="index"></param>

        if($scope.alternativeWorkDistances[index] != undefined){
            if ($scope.alternativeWorkDistances[index].toString().indexOf(".") > -1 || $scope.alternativeWorkDistances[index].toString().indexOf(",") > -1) {
                  // Show popup if distance contains , or .
                  NotificationService.AutoFadeNotification("warning", "", "Afvigende km på ikke indeholde komma eller punktum.");
                  return;
            }
        }
        if(isAddressSet(index)){
            handleSavingAlternativeAddress(index);
        }
        handleSavingAlternativeDistance(index);

    }


    $scope.clearWorkClicked = function (index) {
        /// <summary>
        /// Clears alternative work address.
        /// </summary>
        /// <param name="index"></param>
        PersonEmployments.patchEmployment({ id: $scope.employments[index].Id }, {
            WorkDistanceOverride: 0,
            AlternativeWorkAddress: null,
            AlternativeWorkAddressId: null,
        }).$promise.then(function () {
            workAddressDirty[index] = false;
            workDistanceDirty[index] = false;
            $scope.alternativeWorkDistances[index] = 0;
            $scope.alternativeWorkAddresses[index] = "";
            if ($scope.employments[index].AlternativeWorkAddressId != null) {
                Address.delete({ id: $scope.employments[index].AlternativeWorkAddressId }).$promise.then(function () {
                    $rootScope.$emit('PersonalAddressesChanged');
                    $scope.employments[index].AlternativeWorkAddress = null;
                    $scope.employments[index].AlternativeWorkAddressId = null;
                    NotificationService.AutoFadeNotification("success", "", "Afvigende afstand og adresse slettet.");
                });
            } else {
                $rootScope.$emit('PersonalAddressesChanged');
                $scope.employments[index].AlternativeWorkAddress = null;
                $scope.employments[index].AlternativeWorkAddressId = null;
                NotificationService.AutoFadeNotification("success", "", "Afvigende afstand og adresse slettet.");
            }

        });


    }

    $scope.saveAlternativeHomeAddress = function () {
        $timeout(function () {
            handleSaveAltHome();
        });
    }

    var handleSaveAltHome = function () {
        /// <summary>
        /// Handles saving alternative home address.
        /// </summary>
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
                }).$promise.then(function () {
                    NotificationService.AutoFadeNotification("success", "", "Afvigende hjemmeadresse redigeret.");
                    homeAddressIsDirty = false;
                    $rootScope.$emit('PersonalAddressesChanged');
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
                }).$promise.then(function (res) {
                    $scope.alternativeHomeAddress = res;
                    $scope.alternativeHomeAddress.string = res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town;
                    NotificationService.AutoFadeNotification("success", "", "Afvigende hjemmeadresse oprettet.");
                    homeAddressIsDirty = false;
                    $rootScope.$emit('PersonalAddressesChanged');
                });
            }
        } else if ($scope.alternativeHomeAddress.string == "" && $scope.alternativeHomeAddress.Id != undefined) {
            $scope.clearHomeClicked();
        }
    }

    $scope.clearHomeClicked = function () {
        /// <summary>
        /// Clears alternative home address.
        /// </summary>
        $scope.alternativeHomeAddress.string = "";
        if ($scope.alternativeHomeAddress.Id != undefined) {
            PersonalAddress.delete({ id: $scope.alternativeHomeAddress.Id }).$promise.then(function () {
                $scope.alternativeHomeAddress = null;
                NotificationService.AutoFadeNotification("success", "", "Afvigende hjemmeadresse slettet.");
                $rootScope.$emit('PersonalAddressesChanged');
            });
        } else {
            NotificationService.AutoFadeNotification("success", "", "Afvigende hjemmeadresse slettet.");
        }
    }

    $scope.homeAddressChanged = function () {
        homeAddressIsDirty = true;
    }

    var checkShouldPrompt = function () {
        /// <summary>
        /// Return true if there are unsaved changes on the page. 
        /// </summary>
        var returnVal = false;
        if ($scope.alternativeHomeAddress != undefined) {
            if (homeAddressIsDirty === true && $scope.alternativeHomeAddress.string != $scope.alternativeHomeAddress.StreetName + " " + $scope.alternativeHomeAddress.StreetNumber + ", " + $scope.alternativeHomeAddress.ZipCode + " " + $scope.alternativeHomeAddress.Town) {
                returnVal = true;
            }
        }
        angular.forEach(workAddressDirty, function (value, key) {
            if (value == true) {
                returnVal = true;
            }
        });
        angular.forEach(workDistanceDirty, function (value, key) {
            if (value == true) {
                returnVal = true;
            }
        });
        return returnVal;
    }

    // Alert the user when navigating away from the page if there are unsaved changes.
    $scope.$on('$stateChangeStart', function (event) {
        if (checkShouldPrompt() === true) {
            var answer = confirm("Du har lavet ændringer på siden, der ikke er gemt. Ønsker du at kassere disse ændringer?");
            if (!answer) {
                event.preventDefault();
            }
        }
    });

    window.onbeforeunload = function (e) {
        if (checkShouldPrompt() === true) {
            return "Du har lavet ændringer på siden, der ikke er gemt. Ønsker du at kassere disse ændringer?";
        }
    };

    $scope.$on('$destroy', function () {
        /// <summary>
        /// Unregister refresh event handler when leaving the page.
        /// </summary>
        window.onbeforeunload = undefined;
    });

    $scope.SmartAddress = SmartAdresseSource;

}]);