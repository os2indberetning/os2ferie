﻿angular.module("application").controller("DrivingController", [
    "$scope", "Person", "PersonEmployments", "Rate", "LicensePlate", "PersonalRoute", "DriveReport", "Address", "SmartAdresseSource", "AddressFormatter", "$q", "ReportId", "$timeout", "NotificationService", "PersonalAddress", "$rootScope", "$modalInstance", "HelpText", "$window",
    function ($scope, Person, PersonEmployments, Rate, LicensePlate, PersonalRoute, DriveReport, Address, SmartAdresseSource, AddressFormatter, $q, ReportId, $timeout, NotificationService, PersonalAddress, $rootScope, $modalInstance, HelpText, $window) {

        HelpText.get({ id: "ReadReportCommentHelp" }).$promise.then(function (res) {
            $scope.ReadReportCommentHelp = res.text;
        });

        HelpText.get({ id: "PurposeHelpText" }).$promise.then(function (res) {
            $scope.PurposeHelpText = res.text;
        });

        HelpText.get({ id: "FourKmRuleHelpText" }).$promise.then(function (res) {
            $scope.fourKmRuleHelpText = res.text;
        });

        // Setup functions in scope.
        $scope.Number = Number;
        $scope.toString = toString;
        $scope.replace = String.replace;


        var isFormDirty = false;

        var isEditingReport = ReportId > 0;
        $scope.container = {};
        $scope.container.datePickerMaxDate = new Date();
        $scope.isEditingReport = isEditingReport;
        var kendoPromise = $q.defer();
        var loadingPromises = [kendoPromise.promise];

        $scope.canSubmitDriveReport = true;

        var mapChanging = false;


        $scope.container.addressFieldOptions = {
            select: function () {
                $timeout(function () {
                    $scope.addressInputChanged();
                });
            }
        }

        $scope.addressPlaceholderText = "Eller indtast adresse her";
        $scope.addressDropDownPlaceholderText = "Vælg fast adresse";
        $scope.SmartAddress = SmartAdresseSource;
        $scope.IsRoute = false;

        // Is set to actually contain something once data has been loaded from backend.
        $scope.validateInput = function () { };

        var setupForNewReport = function () {
            /// <summary>
            /// Initializes fields for new report.
            /// </summary>
            $scope.DriveReport = new DriveReport();
            $scope.DriveReport.Addresses = [];
            $scope.DriveReport.Addresses.push({ Name: "", Personal: "" });
            $scope.DriveReport.Addresses.push({ Name: "", Personal: "" });
            // Set the datepicker date to today.
            $scope.DriveReport.Date = new Date();
        }

        setupForNewReport();

        $scope.AddViapoint = function () {
            /// <summary>
            /// Adds via point
            /// </summary>
            var temp = $scope.DriveReport.Addresses.pop();
            $scope.DriveReport.Addresses.push({ Name: "", Personal: "", Save: false });
            $scope.DriveReport.Addresses.push(temp);
        };

        $scope.Remove = function (array, index) {
            /// <summary>
            /// Removes via point
            /// </summary>
            /// <param name="array"></param>
            /// <param name="index"></param>
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
            /// <summary>
            /// Removes commas from Address string from Septima.
            /// Septima addresses are in the format                 "StreetName StreetNumber, ZipCode, Town"
            /// Addresses used in the app need to be in the format  "StreetName StreetNumber, Zipcode Town"
            /// </summary>
            /// <param name="address"></param>
            var res = address.toString().replace(/,/, "###");
            res = res.replace(/,/g, "");
            res = res.replace(/###/, ",");
            return res;
        }

        var getCurrentUserEmployment = function (employmentId) {
            /// <summary>
            /// Gets employment for current user.
            /// </summary>
            /// <param name="employmentId"></param>
            var res;
            angular.forEach($scope.currentUser.Employments, function (empl, key) {
                if (empl.Id == employmentId) {
                    res = empl;
                }
            });
            return res;
        }


        var loadValuesFromReport = function (report) {
            /// <summary>
            /// Loads values from user's latest report and sets fields in the view.
            /// </summary>
            /// <param name="report"></param>
            $scope.DriveReport.FourKmRule = {};
            $scope.DriveReport.FourKmRule.Value = $rootScope.CurrentUser.DistanceFromHomeToBorder.toString().replace(".", ",");

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
                $scope.DriveReport.FourKmRule.Using = report.FourKmRule;
                $scope.DriveReport.Date = moment.unix(report.DriveDateTimestamp)._d;

                if (report.KilometerAllowance == "Read") {
                    $scope.DriveReport.ReadDistance = report.Distance.toString().replace(".", ",");
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
                    if (value.Description != "") {
                        value.PresentationString = value.Plate + " - " + value.Description;
                    } else {
                        value.PresentationString = value.Plate;
                    }
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
                if (value.Type == "Home") {
                    // Store home address
                    $scope.HomeAddress = value;
                    value.PresentationString += "Hjemmeadresse : ";
                }
                if (value.Type == "AlternativeHome") {
                    // Overwrite home address if user has alternative home address.
                    $scope.HomeAddress = value;
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
            /// <summary>
            /// Sets fields for report to be not a personal route.
            /// </summary>
            $scope.container.PersonalRouteDropDown.select(0);
            $scope.IsRoute = false;
            $scope.DriveReport.Addresses = [{ Name: "" }, { Name: "" }];
        }

        var setIsRoute = function (index) {
            /// <summary>
            /// Sets field for report to be a personal route.
            /// </summary>
            /// <param name="index"></param>
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
            isFormDirty = true;
        }

        $scope.personalRouteDropdownChange = function (e) {
            /// <summary>
            /// Event handler for personal route dropdown.
            /// </summary>
            /// <param name="e"></param>
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

        var validateDate = function () {
            if ($scope.DriveReport.Date == null || $scope.DriveReport.Date == undefined) {
                return false;
            }
            return true;
        }

        var validatePurpose = function () {
            /// <summary>
            /// Validates purposes and sets error message in view accordingly.
            /// </summary>
            $scope.purposeErrorMessage = "";
            if ($scope.DriveReport.Purpose == undefined || $scope.DriveReport.Purpose == "") {
                $scope.purposeErrorMessage = "* Du skal angive et formål.";
                return false;
            }
            return true;
        }

        var validateFourKmRule = function () {
            /// <summary>
            /// Validates fourkmrule and sets error message in view accordingly.
            /// </summary>
            $scope.fourKmRuleValueErrorMessage = "";
            if ($scope.DriveReport.FourKmRule.Using === true && ($scope.DriveReport.FourKmRule.Value == "" || $scope.DriveReport.FourKmRule.Value == undefined)) {
                $scope.fourKmRuleValueErrorMessage = "* Du skal udfylde en 4 km-regel værdi.";
                return false;
            }
            return true;
        }

        var validateLicensePlate = function () {
            /// <summary>
            /// Validates license plate and sets error message in view accordingly.
            /// </summary>
            $scope.licensePlateErrorMessage = "";
            if (getKmRate($scope.DriveReport.KmRate).Type.RequiresLicensePlate && $scope.LicensePlates[0].PresentationString == "Ingen nummerplader") {
                $scope.licensePlateErrorMessage = "* Det valgte transportmiddel kræver en nummerplade.";
                return false;
            }
            return true;
        }

        var validateReadInput = function () {
            /// <summary>
            /// Validates Read Report driven distance and sets eror message in view accordingly.
            /// </summary>
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
            /// <summary>
            /// Resolves address coordinates and updates map.
            /// </summary>
            /// <param name="index"></param>
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
                isFormDirty = true;
            });
        }

        var setMap = function (mapArray) {
            /// <summary>
            /// Updates the map widget in the view.
            /// </summary>
            /// <param name="mapArray"></param>
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
            /// <summary>
            /// Creates the map widget in the view.
            /// </summary>
            $timeout(function () {
                if (angular.element('#map').length) {
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
                }
            });
        }



        var dataAndKendoLoaded = function () {
            /// <summary>
            ///  Is called when Kendo has rendered up to and including KilometerAllowanceDropDown and data has been loaded from backend.
            /// Consider this function Main()
            /// Is needed to make sure data and kendo widgets are ready for setting values from previous drivereport.
            /// </summary>

            // Define validateInput now. Otherwise it gets called from drivingview.html before having loaded resources.
            $scope.validateInput = function () {
                $timeout(function () {
                    $scope.canSubmitDriveReport = validateReadInput();
                    $scope.canSubmitDriveReport &= validateAddressInput();
                    $scope.canSubmitDriveReport &= validatePurpose();
                    $scope.canSubmitDriveReport &= validateLicensePlate();
                    $scope.canSubmitDriveReport &= validateFourKmRule();
                    $scope.canSubmitDriveReport &= validateDate();
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
            /// <summary>
            /// Clears user input
            /// </summary>

            isFormDirty = false;

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

        var handleSave = function () {
            /// <summary>
            /// Handles saving of drivereport.
            /// </summary>
            $scope.canSubmitDriveReport = false;
            if (isEditingReport) {
                DriveReport.delete({ id: ReportId }).$promise.then(function () {
                    DriveReport.edit($scope).$promise.then(function (res) {
                        $scope.latestDriveReport = res;
                        NotificationService.AutoFadeNotification("success", "", "Din tjenestekørselsindberetning blev redigeret");
                        $scope.clearClicked();
                        $modalInstance.close();
                        $scope.container.driveDatePicker.close();
                    }, function () {
                        NotificationService.AutoFadeNotification("danger", "", "Der opstod en fejl under redigering af tjenestekørselsindberetningen.");
                    });
                });
            } else {
                DriveReport.create($scope).$promise.then(function (res) {
                    $scope.latestDriveReport = res;
                    NotificationService.AutoFadeNotification("success", "", "Din indberetning er sendt til godkendelse.");
                    $scope.clearClicked();
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "", "Der opstod en fejl under oprettelsen af tjenestekørselsindberetningen.");
                });
            }
        }

        $scope.Save = function () {
            if (!$scope.canSubmitDriveReport) {
                return;
            }

            if ($rootScope.CurrentUser.DistanceFromHomeToBorder != $scope.DriveReport.FourKmRule.Value && $scope.DriveReport.FourKmRule.Value != "" && $scope.DriveReport.FourKmRule.Value != undefined) {
                $rootScope.CurrentUser.DistanceFromHomeToBorder = $scope.DriveReport.FourKmRule.Value
                Person.patch({ id: $rootScope.CurrentUser.Id }, { DistanceFromHomeToBorder: $scope.DriveReport.FourKmRule.Value.toString().replace(",", ".") }).$promise.then(function () {
                    handleSave();
                });
            } else {
                handleSave();
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
                    $scope.hasAccessToFourKmRule = empl.OrgUnit.HasAccessToFourKmRule;
                }
            });
            updateDrivenKm();
            $scope.validateInput();
        }

        var routeStartsAtHome = function () {
            /// <summary>
            /// returns true if route starts at home
            /// </summary>
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
                var endAddress = $scope.currentMapAddresses[0];
                return areAddressesCloseToEachOther($scope.HomeAddress, endAddress);
            }
        }

        var routeEndsAtHome = function () {
            /// <summary>
            /// Returns true if route ends at home.
            /// </summary>
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
                var endAddress = $scope.currentMapAddresses[$scope.currentMapAddresses.length - 1];
                return areAddressesCloseToEachOther($scope.HomeAddress, endAddress);
            }
        }

        //Checks that two addresses are within 100 meters, in
        //which case we assume they are the same when regarding
        //if a person starts or ends their route at home.
        var areAddressesCloseToEachOther = function (address1, address2) {
            //Longitude and latitude is called different things depending on
            //whether we get the information from the backend or from septima
            var long1 = (address1.Longitude === undefined) ? address1.lng : address1.Longitude;
            var long2 = (address2.Longitude === undefined) ? address2.lng : address2.Longitude;
            var lat1 = (address1.Latitude === undefined) ? address1.lat : address1.Latitude;
            var lat2 = (address2.Latitude === undefined) ? address2.lat : address2.Latitude;

            var longDiff = Math.abs(Number(long1) - Number(long2));
            var latDiff = Math.abs(Number(lat1) - Number(lat2));
            return longDiff < 0.0001 && latDiff < 0.001; //Fourth decimal is ~10 meters
        }

        $scope.startEndHomeChanged = function () {
            updateDrivenKm();
        }

        var updateDrivenKm = function () {
            /// <summary>
            /// Updates drivenkm fields under map widget.
            /// </summary>
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
                $scope.DrivenKMDisplay = Number($scope.DriveReport.ReadDistance.toString().replace(",", "."));
            } else {
                if ($scope.latestMapDistance == undefined) {
                    $scope.DrivenKMDisplay = 0;
                } else {
                    $scope.DrivenKMDisplay = $scope.latestMapDistance;
                }
            }

            if ($scope.DriveReport.RoundTrip === true) {
                // Double the driven km if its a roundtrip.
                $scope.DrivenKMDisplay = Number($scope.DrivenKMDisplay) * 2;
                // If the route starts xor ends at home -> double the transportallowance.
                // The case where the route both ends and starts at home is already covered.
                if (routeStartsAtHome() != routeEndsAtHome()) {

                    $scope.TransportAllowance = Number($scope.TransportAllowance) * 2;
                }
            }
        }

        $scope.readDistanceChanged = function () {
            updateDrivenKm();
        }

        $scope.roundTripChanged = function () {
            updateDrivenKm();
        }

        $scope.closeModalWindow = function () {
            $modalInstance.dismiss();
        }


        var handleDiscardChanges = function (event) {
            /// <summary>
            /// Prompts the user when he/she attempts to leave a page with unsaved changes.
            /// </summary>
            /// <param name="event"></param>
            var returnVal = undefined;
            if (isFormDirty === true ||
               ($scope.DriveReport.Purpose != $scope.latestDriveReport.Purpose && $scope.DriveReport.Purpose != "") ||
               ($scope.DriveReport.ReadDistance != $scope.latestDriveReport.Distance.toString().replace(".", ",") && $scope.DriveReport.ReadDistance != "") ||
               ($scope.DriveReport.UserComment != undefined && $scope.DriveReport.UserComment != $scope.latestDriveReport.UserComment && $scope.DriveReport.UserComment != "")) {
                var answer = confirm("Du har lavet ændringer på siden, der ikke er gemt. Ønsker du at kassere disse ændringer?");
                if (!answer) {
                    returnVal = "Du har lavet ændringer på siden, der ikke er gemt. Ønsker du at kassere disse ændringer?";
                    event.preventDefault();
                }
            }
            return returnVal;
        }

        // Alert the user when navigating away from the page if there are unsaved changes.
        $scope.$on('$stateChangeStart', function (event) {
            handleDiscardChanges(event);
        });

        window.onbeforeunload = function (e) {
            return handleDiscardChanges(e);
        };

        $scope.$on('$destroy', function () {
            window.onbeforeunload = undefined;
        });
    }
]);
