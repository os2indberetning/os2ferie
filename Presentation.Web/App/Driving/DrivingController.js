angular.module("application").controller("DrivingController", [
    "$scope", "Person", "PersonEmployments", "Rate", "LicensePlate", "PersonalRoute", "DriveReport", "Address", "SmartAdresseSource", "AddressFormatter", "$q", "ReportId", "$timeout", "NotificationService",
    function ($scope, Person, PersonEmployments, Rate, LicensePlate, PersonalRoute, DriveReport, Address, SmartAdresseSource, AddressFormatter, $q, ReportId, $timeout, NotificationService) {

        var isEditingReport = ReportId > 0;

        $scope.container = {};
        var kendoPromise = $q.defer();
        var WaitingPromise = $q.defer();
        var loadingPromises = [WaitingPromise.promise, kendoPromise.promise];

        $scope.canSubmitDriveReport = true;

        var mapChanging = false;


        $scope.addressPlaceholderText = "Eller indtast adresse her";
        $scope.addressDropDownPlaceholderText = "Vælg fast adresse";
        $scope.SmartAddress = SmartAdresseSource;
        $scope.IsRoute = false;


        var setupForNewReport = function () {
            $scope.DriveReport = new DriveReport();
            $scope.DriveReport.Addresses = [];
            $scope.DriveReport.Addresses.push({ Name: "", Personal: "" });
            $scope.DriveReport.Addresses.push({ Name: "", Personal: "" });
            // Set the datepicker date to today.
            $scope.DriveReport.Date = new Date();
        }


        setupForNewReport();


        $scope.AddViapoint = function () {
            var temp = $scope.DriveReport.Addresses.pop();
            $scope.DriveReport.Addresses.push({ Name: "", Personal: "", Save: false });
            $scope.DriveReport.Addresses.push(temp);
        };

        $scope.Remove = function (array, index) {
            array.splice(index, 1);
            $scope.addressInputChanged(index);
        };

        var getKmRate = function () {
            for (var i = 0; i < $scope.KmRate.length; i++) {
                if ($scope.KmRate[i].Id == $scope.DriveReport.KmRate) {
                    return $scope.KmRate[i];
                }
            }
        };

        $scope.shaveExtraCommasOffAddressString = function (address) {

            var res = address.toString().replace(/,/, "###");
            res = res.replace(/,/g, "");
            res = res.replace(/###/, ",");
            return res;
        }



        var loadValuesFromReport = function (report) {
            // Select position in dropdown.
            $scope.container.PositionDropDown.select(function (item) {
                return item.Id == report.Employment.Id;
            });

            // Select the right license plate.
            $scope.container.LicensePlateDropDown.select(function (item) {
                return item.Plate == report.LicensePlate;
            });
            $scope.container.LicensePlateDropDown.trigger("change");

            // Select kilometer allowance.
            switch (report.KilometerAllowance) {
                case "Calculated":
                    $scope.container.KilometerAllowanceDropDown.select(0);
                    break;
                case "Read":
                    $scope.container.KilometerAllowanceDropDown.select(1);
                    break;
                case "CalculatedWithoutExtraDistance":
                    $scope.container.KilometerAllowanceDropDown.select(2);
                    break;
            }

            $scope.DriveReport.KilometerAllowance = $scope.container.KilometerAllowanceDropDown._selectedValue;

            // Select KmRate

            $scope.container.KmRateDropDown.select(function (item) {
                return item.Type.TFCode == report.TFCode;
            });

            angular.forEach($scope.KmRate, function (rate, key) {
                if (rate.Type.TFCode == report.TFCode) {
                    $scope.showLicensePlate = rate.Type.RequiresLicensePlate;
                }
            });

            $scope.container.KmRateDropDown.trigger("change");
        }


        // Load all data
        Person.GetCurrentUser().$promise.then(function (currentUser) {

            $scope.currentUser = currentUser;

            // Load user's positions.
            loadingPromises.push(PersonEmployments.get({ id: currentUser.Id }).$promise.then(function (res) {
                angular.forEach(res, function (value, key) {
                    value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription;
                });
                $scope.Employments = res;
            }));

            // Load this year's rates.
            loadingPromises.push(Rate.ThisYearsRates().$promise.then(function (res) {
                $scope.KmRate = res;
            }));

            // Load user's license plates.
            loadingPromises.push(LicensePlate.get({ id: currentUser.Id }).$promise.then(function (res) {
                if (res.length > 0) {
                    angular.forEach(res, function (value, key) {
                        value.PresentationString = value.Plate + " - " + value.Description;
                    });
                    $scope.LicensePlates = res;
                } else {
                    $scope.LicensePlates = [{ PresentationString: "Ingen nummerplader", Plate: "0000000" }];
                }

            }));

            // Load user's personal routes
            loadingPromises.push(PersonalRoute.getForUser({ id: currentUser.Id }).$promise.then(function (res) {
                angular.forEach(res, function (value, key) {
                    value.PresentationString = "";
                    if (value.Description != "") {
                        value.PresentationString += value.Description + " : ";
                    }
                    value.PresentationString += value.Points[0].StreetName + " " + value.Points[0].StreetNumber + ", " + value.Points[0].ZipCode + " " + value.Points[0].Town + " -> ";
                    value.PresentationString += value.Points[value.Points.length - 1].StreetName + " " + value.Points[value.Points.length - 1].StreetNumber + ", " + value.Points[value.Points.length - 1].ZipCode + " " + value.Points[value.Points.length - 1].Town;
                    value.PresentationString += " Antal viapunkter: " + Number(value.Points.length - 2);
                });
                res.unshift({ PresentationString: "Vælg personlig rute" });
                $scope.Routes = res;
            }));

            OS2RouteMap.create({
                id: 'map',
                change: function (obj) {
                    // Return if the change comes from AddressInputChanged
                    if (mapChanging === true) {
                        setMapPromise.resolve();
                        return;
                    }

                    if ($scope.IsRoute) {
                        setNotRoute();
                    }

                    mapChanging = true;
                    $scope.DriveReport.Addresses = [];
                    // Load the adresses from the map.
                    angular.forEach(obj.Addresses, function (address, key) {
                        var shavedName = $scope.shaveExtraCommasOffAddressString(address.name);
                        $scope.DriveReport.Addresses.push({ Name: shavedName, Latitude: address.lat, Longitude: address.lng });
                    });
                    // Apply to update the view.
                    $scope.$apply();
                    $timeout(function () {
                        // Wait for the view to render before setting mapChanging to false.
                        mapChanging = false;
                    });

                }
            });

            if (!isEditingReport) {
                loadingPromises.push(Address.getMapStart().$promise.then(function (res) {
                    setMap(res);
                }));

                loadingPromises.push(DriveReport.getLatest({ id: currentUser.Id }).$promise.then(function (res) {
                    $scope.latestDriveReport = res;
                }));
            } else {
                loadingPromises.push(DriveReport.get({ id: ReportId }).$promise.then(function (res) {
                    $scope.latestDriveReport = res;
                }));
            }



            loadingPromises.push(Address.GetPersonalAndStandard({ personId: currentUser.Id }).$promise.then(function (res) {
                angular.forEach(res, function (value, key) {
                    value.PresentationString = "";
                    if (value.Description != "") {
                        value.PresentationString += value.Description + " : ";
                    }
                    if (value.Type == "Home" || value.Type == "AlternativeHome") {
                        value.PresentationString += "Hjemmeadresse : ";
                    }
                    if (value.Type == "Work" || value.Type == "AlternativeWork") {
                        value.PresentationString += "Arbejdsadresse : ";
                    }

                    value.PresentationString += value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;

                });
                res.unshift({ PresentationString: $scope.addressDropDownPlaceholderText });
                $scope.PersonalAddresses = res;
            }));

            WaitingPromise.resolve();

            $q.all(loadingPromises).then(function (res) {
                dataAndKendoLoaded();
            });
        });

        var getPersonalAddress = function (id) {
            id = Number(id);
            var val;
            angular.forEach($scope.PersonalAddresses, function (value, key) {
                if (value.Id == id) {
                    val = value;
                }
            });
            return val;
        }

        var setNotRoute = function () {
            $scope.container.PersonalRouteDropDown.select(0);
            $scope.IsRoute = false;
            $scope.DriveReport.Addresses = [{ Name: "" }, { Name: "" }];
        }

        var setIsRoute = function (index) {
            $scope.IsRoute = true;
            var route = $scope.Routes[index];
            $scope.DriveReport.Addresses = [];
            var mapArray = [];
            angular.forEach(route.Points, function (address, key) {
                var addr = {
                    Name: address.StreetName + " " + address.StreetNumber + ", " + address.ZipCode + " " + address.Town,
                    Latitude: address.Latitude,
                    Longitude: address.Longitude
                };
                $scope.DriveReport.Addresses.push(addr);
                mapArray.push({ name: addr.Name, lat: addr.Latitude, lng: addr.Longitude });
            });
            mapChanging = true;
            var prom = setMap(mapArray);
            prom.promise.then(function () {
                mapChanging = false;
            });
        }

        $scope.personalRouteDropdownChange = function (e) {
            var index = e.sender.selectedIndex;
            if (index == 0) {
                setNotRoute();
            } else {
                setIsRoute(index);
            }
        }

        $scope.isAddressNameSet = function (address) {
            return !(address.Name == "" || address.Name == $scope.addressPlaceholderText || address.Name == undefined);
        }

        var validateAddressInput = function () {
            if ($scope.DriveReport.KilometerAllowance == "Read") {
                return true;
            }
            var res = true;
            $scope.addressSelectionErrorMessage = "";
            angular.forEach($scope.DriveReport.Addresses, function (address, key) {
                if (!$scope.isAddressNameSet(address) && address.Personal == $scope.addressDropDownPlaceholderText) {
                    res = false;
                    $scope.addressSelectionErrorMessage = "*  Du skal udfylde alle adressefelter.";
                }
            });
            return res;
        }

        var validatePurpose = function () {
            $scope.purposeErrorMessage = "";
            if ($scope.DriveReport.Purpose == undefined || $scope.DriveReport.Purpose == "") {
                $scope.purposeErrorMessage = "* Du skal angive et formål.";
                return false;
            }
            return true;
        }

        var validateLicensePlate = function () {
            $scope.licensePlateErrorMessage = "";
            if (getKmRate($scope.DriveReport.KmRate).Type.RequiresLicensePlate && $scope.LicensePlates[0].PresentationString == "Ingen nummerplader") {
                $scope.licensePlateErrorMessage = "* Det valgte transportmiddel kræver en nummerplade.";
                return false;
            }
            return true;
        }

        var validateReadInput = function () {
            $scope.readDistanceErrorMessage = "";
            $scope.userCommentErrorMessage = "";
            var distRes = true;
            var commRes = true;
            if ($scope.DriveReport.KilometerAllowance == "Read") {
                if ($scope.DriveReport.ReadDistance <= 0 || $scope.DriveReport.ReadDistance == undefined) {
                    $scope.readDistanceErrorMessage = "* Du skal indtaste en kørt afstand.";
                    distRes = false;
                }
                if ($scope.DriveReport.UserComment == undefined || $scope.DriveReport.UserComment == "") {
                    $scope.userCommentErrorMessage = "* Du skal angive en kommentar.";
                    commRes = false;
                }
            }
            return commRes && distRes;
        }

        $scope.validateInput = function () {
            // Each function returns a bool. canSubmitDriveReport is the logical and of each function call.
            // It has to be one & instead of && as all the functions should get called. && will stop calling when the first function returns false.
            $scope.canSubmitDriveReport = (validateReadInput() & validateAddressInput() & validatePurpose() & validateLicensePlate());
        }



        $scope.addressInputChanged = function (index) {
            if (!validateAddressInput() || mapChanging) {
                return;
            }


            var promises = [];
            var mapArray = [];

            angular.forEach($scope.DriveReport.Addresses, function (addr, key) {
                if (!$scope.isAddressNameSet(addr) && addr.Personal != $scope.addressDropDownPlaceholderText) {
                    var pers = getPersonalAddress(addr.Personal);
                    mapArray.push({ lat: pers.Latitude, lng: pers.Longitude });
                } else if ($scope.isAddressNameSet(addr)) {
                    var format = AddressFormatter.fn(addr.Name);
                    promises.push(Address.setCoordinatesOnAddress({ StreetName: format.StreetName, StreetNumber: format.StreetNumber, ZipCode: format.ZipCode, Town: format.Town }).$promise.then(function (res) {
                        addr.Latitude = res[0].Latitude;
                        addr.Longitude = res[0].Longitude;
                        mapArray.push({ name: addr.Name, lat: addr.Latitude, lng: addr.Longitude });
                    }));
                }
            });

            $q.all(promises).then(function (data) {
                mapChanging = true;
                var prom = setMap(mapArray);
                prom.promise.then(function () {
                    mapChanging = false;
                });
            });

        }

        var setMapPromise;
        var setMap = function (mapArray) {
            setMapPromise = $q.defer();
            OS2RouteMap.set(mapArray);
            return setMapPromise;
        }


        // Wait for kendo to render.
        $scope.$on("kendoWidgetCreated", function (event, widget) {
            if (widget === $scope.container.KilometerAllowanceDropDown) {
                kendoPromise.resolve();
            }
        });


        // Is called when Kendo has rendered up to and including KilometerAllowanceDropDown and data has been loaded from backend.
        // Consider this function Main()
        // Is needed to make sure data and kendo widgets are ready for setting values from previous drivereport.
        var dataAndKendoLoaded = function () {

            loadValuesFromReport($scope.latestDriveReport);
            if (isEditingReport) {
            } else {
                $scope.container.driveDatePicker.open();
            }
        }

        $scope.clearClicked = function () {
            $scope.container.driveDatePicker.open();
            loadValuesFromReport($scope.latestDriveReport);
            $scope.DriveReport.Addresses = [{ Name: "" }, { Name: "" }];
            $scope.DriveReport.ReadDistance = 0;
            $scope.DriveReport.UserComment = "";
            $scope.DriveReport.Purpose = "";
            $scope.validateInput();
        }

        $scope.transportChanged = function (res) {
            $q.all(loadingPromises).then(function () {
                validateLicensePlate();
                $scope.showLicensePlate = getKmRate($scope.DriveReport.KmRate).Type.RequiresLicensePlate;
            });
        }

        $scope.Save = function () {
            if (isEditingReport) {
                DriveReport.delete({ id: ReportId }).$promise.then(function () {
                    DriveReport.save($scope).$promise.then(function(res) {
                        $scope.latestDriveReport = res;
                        NotificationService.AutoFadeNotification("success", "", "Din tjenestekørselsindberetning blev redigeret");
                        $scope.clearClicked();
                    });
                });
            } else {
                DriveReport.save($scope).$promise.then(function (res) {
                    $scope.latestDriveReport = res;
                    NotificationService.AutoFadeNotification("success", "", "Din tjenestekørselsindberetning blev gemt");
                    $scope.clearClicked();
                });
            }
        }

    }
]);