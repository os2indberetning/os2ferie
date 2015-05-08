angular.module("application").controller("DrivingController", [
    "$scope", "Person", "PersonEmployments", "Rate", "LicensePlate", "PersonalRoute", "DriveReport", "Address", "SmartAdresseSource", "AddressFormatter", "$q", "ReportId", "$timeout", "NotificationService", "PersonalAddress", "$rootScope", "$modalInstance",
    function ($scope, Person, PersonEmployments, Rate, LicensePlate, PersonalRoute, DriveReport, Address, SmartAdresseSource, AddressFormatter, $q, ReportId, $timeout, NotificationService, PersonalAddress, $rootScope, $modalInstance) {



        // Setup functions in scope.
        $scope.Number = Number;
        $scope.toString = toString;
        $scope.replace = String.replace;

        var isEditingReport = ReportId > 0;
        $scope.container = {};
        $scope.isEditingReport = isEditingReport;
        var kendoPromise = $q.defer();
        var loadingPromises = [kendoPromise.promise];

        $scope.canSubmitDriveReport = true;

        var mapChanging = false;


        $scope.addressPlaceholderText = "Eller indtast adresse her";
        $scope.addressDropDownPlaceholderText = "Vælg fast adresse";
        $scope.SmartAddress = SmartAdresseSource;
        $scope.IsRoute = false;

        // Is set to actually contain something once data has been loaded from backend.
        $scope.validateInput = function () { };

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

        var getCurrentUserEmployment = function (employmentId) {
            var res;
            angular.forEach($scope.currentUser.Employments, function (empl, key) {
                if (empl.Id == employmentId) {
                    res = empl;
                }
            });
            return res;
        }


        var loadValuesFromReport = function (report) {
            // Select position in dropdown.
            $scope.container.PositionDropDown.select(function (item) {
                return item.Id == report.EmploymentId;
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


            // Load additional data if a report is being edited.
            if (isEditingReport) {
                $scope.DriveReport.Purpose = report.Purpose;
                $scope.DriveReport.Date = moment.unix(report.DriveDateTimestamp)._d;

                if (report.KilometerAllowance == "Read") {
                    $scope.DriveReport.ReadDistance = report.Distance.toString().replace(".",",");
                    $scope.DriveReport.UserComment = report.UserComment;
                    if (!report.StartsAtHome && !report.EndsAtHome) {
                        $scope.container.StartEndHomeDropDown.select(0);
                    } else if (report.StartsAtHome && report.EndsAtHome) {
                        $scope.container.StartEndHomeDropDown.select(3);
                    } else if (report.StartsAtHome) {
                        $scope.container.StartEndHomeDropDown.select(1);
                    } else if (report.EndsAtHome) {
                        $scope.container.StartEndHomeDropDown.select(2);
                    }
                    $scope.DriveReport.StartsAtHome = report.StartsAtHome;
                    $scope.DriveReport.EndsAtHome = report.EndsAtHome;
                } else {
                    $scope.DriveReport.Addresses = [];
                    angular.forEach(report.DriveReportPoints, function (point, key) {
                        var temp = { Name: point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town, Latitude: point.Latitude, Longitude: point.Longitude };
                        $scope.DriveReport.Addresses.push(temp);
                    });
                }
            }
            $scope.validateInput();
        }

        // Load all data
        $scope.currentUser = $rootScope.CurrentUser;
        var currentUser = $scope.currentUser;


        // Load user's positions.
        angular.forEach(currentUser.Employments, function (value, key) {
            value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription;
        });
        $scope.Employments = currentUser.Employments;

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

        // Load map start address
        loadingPromises.push(Address.getMapStart().$promise.then(function (res) {
            $scope.mapStartAddress = res;
        }));

        if (!isEditingReport) {
            // Load latest drive report
            loadingPromises.push(DriveReport.getLatest({ id: currentUser.Id }).$promise.then(function (res) {
                $scope.latestDriveReport = res;
            }));
        } else {
            // Load report to be edited.
            loadingPromises.push(DriveReport.getWithPoints({ id: ReportId }).$promise.then(function (res) {
                $scope.latestDriveReport = res;
            }));
        }

        // Load personal and standard addresses.
        loadingPromises.push(Address.GetPersonalAndStandard({ personId: currentUser.Id }).$promise.then(function (res) {
            angular.forEach(res, function (value, key) {
                value.PresentationString = "";
                if (value.Description != "" && value.Description != null && value.Description != undefined) {
                    value.PresentationString += value.Description + " : ";
                }
                if (value.Type == "Home" || value.Type == "AlternativeHome") {
                    $scope.HomeAddress = value;
                    value.PresentationString += "Hjemmeadresse : ";
                }

                value.PresentationString += value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;
                value.address = value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;
            });
            res.unshift({ PresentationString: $scope.addressDropDownPlaceholderText });
            $scope.PersonalAddresses = res;
        }));


        $q.all(loadingPromises).then(function (res) {
            dataAndKendoLoaded();
        });

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
            setMap(mapArray);
        }

        $scope.personalRouteDropdownChange = function (e) {
            var index = e.sender.selectedIndex;
            if (index == 0) {
                setNotRoute();
            } else {
                setIsRoute(index);
            }
            $scope.validateInput();
        }

        $scope.isAddressNameSet = function (address) {
            return !(address.Name == "" || address.Name == $scope.addressPlaceholderText || address.Name == undefined);
        }

        $scope.isAddressPersonalSet = function (address) {
            return !(address.Personal == "" || address.Personal == $scope.addressDropDownPlaceholderText || address.Personal == undefined);
        }

        var validateAddressInput = function () {
            if ($scope.DriveReport.KilometerAllowance == "Read") {
                return true;
            }
            var res = true;
            $scope.addressSelectionErrorMessage = "";
            angular.forEach($scope.DriveReport.Addresses, function (address, key) {
                if (!$scope.isAddressNameSet(address) && !$scope.isAddressPersonalSet(address)) {
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

        

        $scope.addressInputChanged = function (index) {
            if (!validateAddressInput() || mapChanging) {
                return;
            }

            $scope.validateInput();
            var promises = [];
            var mapArray = [];

            angular.forEach($scope.DriveReport.Addresses, function (addr, key) {
                if (!$scope.isAddressNameSet(addr) && addr.Personal != $scope.addressDropDownPlaceholderText) {
                    var format = AddressFormatter.fn(addr.Personal);
                    promises.push(Address.setCoordinatesOnAddress({ StreetName: format.StreetName, StreetNumber: format.StreetNumber, ZipCode: format.ZipCode, Town: format.Town }).$promise.then(function (res) {
                        addr.Latitude = res[0].Latitude;
                        addr.Longitude = res[0].Longitude;
                        mapArray[key] = { name: addr.Name, lat: addr.Latitude, lng: addr.Longitude };
                    }));
                } else if ($scope.isAddressNameSet(addr)) {
                    var format = AddressFormatter.fn(addr.Name);
                    promises.push(Address.setCoordinatesOnAddress({ StreetName: format.StreetName, StreetNumber: format.StreetNumber, ZipCode: format.ZipCode, Town: format.Town }).$promise.then(function (res) {
                        addr.Latitude = res[0].Latitude;
                        addr.Longitude = res[0].Longitude;
                        mapArray[key] = { name: addr.Name, lat: addr.Latitude, lng: addr.Longitude };
                    }));
                }
            });

            $q.all(promises).then(function (data) {
                setMap(mapArray);
            });
        }

        var setMap = function (mapArray) {
            $timeout(function () {
                setMapPromise = $q.defer();
                mapChanging = true;
                OS2RouteMap.set(mapArray);

                setMapPromise.promise.then(function () {
                    mapChanging = false;
                });
            });
        }

        // Wait for kendo to render.
        $scope.$on("kendoWidgetCreated", function (event, widget) {
            if (widget === $scope.container.KilometerAllowanceDropDown) {
                kendoPromise.resolve();
            }
        });

        var createMap = function () {
            $timeout(function () {
                OS2RouteMap.create({
                    id: 'map',
                    change: function (obj) {
                        $scope.currentMapAddresses = obj.Addresses;
                        $scope.latestMapDistance = obj.distance;
                        updateDrivenKm();

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
                OS2RouteMap.set($scope.mapStartAddress);
            });
        }


        // Is called when Kendo has rendered up to and including KilometerAllowanceDropDown and data has been loaded from backend.
        // Consider this function Main()
        // Is needed to make sure data and kendo widgets are ready for setting values from previous drivereport.
        var dataAndKendoLoaded = function () {

            // Define validateInput now. Otherwise it gets called from drivingview.html before having loaded resources.
            $scope.validateInput = function () {
                $timeout(function () {
                    $scope.canSubmitDriveReport = validateReadInput();
                    $scope.canSubmitDriveReport &= validateAddressInput();
                    $scope.canSubmitDriveReport &= validatePurpose();
                    $scope.canSubmitDriveReport &= validateLicensePlate();
                });
            }

            if (!isEditingReport) {
                $scope.container.driveDatePicker.open();
            }

            // Timeout for wait for dom to render.
            $timeout(function () {
                createMap();
                loadValuesFromReport($scope.latestDriveReport);
                updateDrivenKm();
            });

        }

        $scope.clearClicked = function () {

            if (!isEditingReport) {
                setMap($scope.mapStartAddress);
            }

            setNotRoute();

            $scope.container.driveDatePicker.open();
            loadValuesFromReport($scope.latestDriveReport);
            $scope.DriveReport.Addresses = [{ Name: "" }, { Name: "" }];
            $scope.DriveReport.ReadDistance = 0;
            $scope.DriveReport.UserComment = "";
            $scope.DriveReport.Purpose = "";
            $scope.validateInput();
            updateDrivenKm();
        }

        $scope.transportChanged = function (res) {
            $q.all(loadingPromises).then(function () {
                $scope.validateInput();
                $scope.showLicensePlate = getKmRate($scope.DriveReport.KmRate).Type.RequiresLicensePlate;
            });
        }

        $scope.Save = function () {
            if (!$scope.canSubmitDriveReport) {
                return;
            }
            if (isEditingReport) {
                DriveReport.delete({ id: ReportId }).$promise.then(function () {
                    DriveReport.edit($scope).$promise.then(function (res) {
                        $scope.latestDriveReport = res;
                        NotificationService.AutoFadeNotification("success", "", "Din tjenestekørselsindberetning blev redigeret");
                        $scope.clearClicked();
                        $modalInstance.close();
                    }, function () {
                        NotificationService.AutoFadeNotification("danger", "", "Der opstod en fejl under redigering af tjenestekørselsindberetningen.");
                    });
                });
            } else {
                DriveReport.create($scope).$promise.then(function (res) {
                    $scope.latestDriveReport = res;
                    NotificationService.AutoFadeNotification("success", "", "Din tjenestekørselsindberetning blev gemt");
                    $scope.clearClicked();
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "", "Der opstod en fejl under oprettelsen af tjenestekørselsindberetningen.");
                });
            }
        }

        $scope.kilometerAllowanceChanged = function () {
            updateDrivenKm();
            switch ($scope.DriveReport.KilometerAllowance) {
                case "Read":
                    setMap($scope.mapStartAddress);
                    break;
                default:
                    $scope.addressInputChanged();
                    break;
            }
            $scope.validateInput();

        }

        $scope.employmentChanged = function () {
            angular.forEach($scope.currentUser.Employments, function (empl, key) {
                if (empl.Id == $scope.DriveReport.Position) {
                    $scope.WorkAddress = empl.OrgUnit.Address;
                }
            });
            updateDrivenKm();
            $scope.validateInput();
        }

        var routeStartsAtHome = function () {
            if ($scope.DriveReport.KilometerAllowance == "Read") {
                var index = $scope.container.StartEndHomeDropDown.selectedIndex;
                if (index == 1 || index == 3) {
                    return true;
                }
                return false;
            } else {
                if ($scope.currentMapAddresses == undefined) {
                    return false;
                }
                var homeAddressString = $scope.HomeAddress.StreetName + " " + $scope.HomeAddress.StreetNumber + ", " + $scope.HomeAddress.ZipCode;
                var res = $scope.currentMapAddresses[0].name.indexOf(homeAddressString);
                return res > -1;
            }
        }

        var routeEndsAtHome = function () {
            if ($scope.DriveReport.KilometerAllowance == "Read") {
                var index = $scope.container.StartEndHomeDropDown.selectedIndex;
                if (index == 2 || index == 3) {
                    return true;
                }
                return false;
            } else {
                if ($scope.currentMapAddresses == undefined) {
                    return false;
                }
                var homeAddressString = $scope.HomeAddress.StreetName + " " + $scope.HomeAddress.StreetNumber + ", " + $scope.HomeAddress.ZipCode;
                var res = $scope.currentMapAddresses[$scope.currentMapAddresses.length - 1].name.indexOf(homeAddressString);
                return res > -1;
            }
        }

        $scope.startEndHomeChanged = function () {
            updateDrivenKm();
        }

        var updateDrivenKm = function () {
            if ($scope.DriveReport.KilometerAllowance != "CalculatedWithoutExtraDistance") {
                if (routeStartsAtHome() && routeEndsAtHome()) {
                    $scope.TransportAllowance = Number(getCurrentUserEmployment($scope.DriveReport.Position).HomeWorkDistance) * 2;
                } else if (routeStartsAtHome() || routeEndsAtHome()) {
                    $scope.TransportAllowance = getCurrentUserEmployment($scope.DriveReport.Position).HomeWorkDistance;
                } else {
                    $scope.TransportAllowance = 0;
                }
            } else {
                $scope.TransportAllowance = 0;
            }

            if ($scope.DriveReport.KilometerAllowance == "Read") {
                if ($scope.DriveReport.ReadDistance == undefined) {
                    $scope.DriveReport.ReadDistance = 0;
                }
                $scope.DrivenKMDisplay = Number($scope.DriveReport.ReadDistance.toString().replace(",","."));
            } else {
                if ($scope.latestMapDistance == undefined) {
                    $scope.DrivenKMDisplay = 0;
                } else {
                    $scope.DrivenKMDisplay = $scope.latestMapDistance;
                }
            }
        }

        $scope.readDistanceChanged = function () {
            updateDrivenKm();
        }
    }
]);
