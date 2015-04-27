angular.module("application").controller("DrivingController", [
    "$scope", "SmartAdresseSource", "DriveReport", "AddressFormatter", "PersonalAddressType", "PersonEmployments", "ThisYearsRates", "NotificationService", "$modal", "$state", "Address", "$q", "$timeout", "ReadReportCommentHelp", "Person", "HomeAddress", "LatestDriveReport", "LicensePlates", "PersonalAndStandardAddresses", "Routes", "MapStartAddress",
    function ($scope, SmartAdresseSource, DriveReport, AddressFormatter, PersonalAddressType, PersonEmployments, ThisYearsRates, NotificationService, $modal, $state, Address, $q, $timeout, ReadReportCommentHelp, Person, HomeAddress, LatestDriveReport, LicensePlates, PersonalAndStandardAddresses, Routes, MapStartAddress) {
        $scope.container = {};
        $scope.KmRate = ThisYearsRates;
        $scope.ReadReportCommentHelp = ReadReportCommentHelp;
        $scope.LicensePlates = LicensePlates;
        $scope.DrivenKMDisplay = 0;
        $scope.addressPlaceholderText = 'Eller skriv adresse her';
        $scope.Routes = Routes;
        $scope.TransportAllowance = Person.DistanceFromHomeToWork.toFixed(2).toString().replace('.', ',');
        $scope.PersonalAddresses = PersonalAndStandardAddresses;
        $scope.SmartAddress = SmartAdresseSource;
        $scope.Employments = PersonEmployments;

        $scope.FourKmRule = {}
        $scope.FourKmRule.Using = false;

        $scope.DriveReport = new DriveReport();

        $scope.DriveReport.Addresses = [];
        $scope.DriveReport.Addresses.push({ Name: "", Personal: "", Save: false });
        $scope.DriveReport.Addresses.push({ Name: "", Personal: "", Save: false });
        $scope.DriveReport.Date = new Date();

        $scope.DateOptions = {
            start: "month"
        };

        $scope.DrivenKilometers = 0;
        $scope.TransportAllowance = 0;
        $scope.RemainingKilometers = 0;





        // Hardcoded personId
        var personId = 1;

        // Magic variable. Is checked when calling generateMapWidget to make sure it is only called when we manually change the gui. IE. not by changes on the map.
        // When the map is changes by the map, the variable is set to the number of address points and is decremented by one for each time a gui element changes
        // which it does once for each address.
        // Simply put: if the var is <= 0 then the map will be drawn.
        $scope.guiChangedByMap = 0;


        $scope.canSubmitDriveReport = true;

        $scope.IsRoute = false;

        $scope.personalRouteDropdownChange = function (e) {
            var index = e.sender.selectedIndex;

            if (index == 0) {
                $scope.IsRoute = false;
                $scope.DriveReport.Addresses = [];

                $scope.DriveReport.Addresses.push({ Personal: "", Name: "", Save: false });
                $scope.DriveReport.Addresses.push({ Personal: "", Name: "", Save: false });
                return;
            }

            $scope.IsRoute = true;

            $scope.DriveReport.Addresses = [];

            angular.forEach(Routes[index].Points, function (value, key) {
                $scope.DriveReport.Addresses.push({ Personal: "", Name: value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town, Save: false });
            });
            $scope.validateInput();
        }

        var getKmRate = function () {
            for (var i = 0; i < $scope.KmRate.length; i++) {
                if ($scope.KmRate[i].Id == $scope.DriveReport.KmRate) {
                    return $scope.KmRate[i];
                }
            }
        };

        $scope.transportChanged = function () {
            $scope.showLicensePlate = true;
            angular.forEach($scope.KmRate, function (rate, key) {
                if ($scope.DriveReport.KmRate == rate.Id) {
                    $scope.showLicensePlate = rate.Type.RequiresLicensePlate;
                }
            });
            $scope.validateInput();
        }

        $scope.validateInput = function () {
            $scope.drivenKmChanged();
            $scope.canSubmitDriveReport = true;
            $scope.purposeErrorMessage = "";
            $scope.readDistanceErrorMessage = "";
            $scope.addressSelectionErrorMessage = "";
            $scope.userCommentErrorMessage = "";
            $scope.licensePlateErrorMessage = "";
            if ($scope.DriveReport.KilometerAllowance === "Read") {
                if ($scope.DriveReport.Purpose == "" || $scope.DriveReport.Purpose == undefined) {
                    $scope.canSubmitDriveReport = false;
                    $scope.purposeErrorMessage = "* Du skal angive et formål.";
                }
                if ($scope.DriveReport.ReadDistance === "" || $scope.DriveReport.ReadDistance == undefined) {
                    $scope.canSubmitDriveReport = false;
                    $scope.readDistanceErrorMessage = "* Du skal angive en afstand.";
                }
                if ($scope.DriveReport.UserComment === "" || $scope.DriveReport.UserComment == undefined) {
                    $scope.canSubmitDriveReport = false;
                    $scope.userCommentErrorMessage = "* Du skal angive en kommentar";
                }
            } else {
                angular.forEach($scope.DriveReport.Addresses, function (address, key) {
                    if ($scope.isAddressNameSet(address) === false && address.Personal == "Vælg fast adresse") {
                        $scope.canSubmitDriveReport = false;
                        $scope.addressSelectionErrorMessage = "* Du skal udfylde alle adressefelter.";
                    }
                });
                if ($scope.DriveReport.Purpose == "" || $scope.DriveReport.Purpose == undefined) {
                    $scope.canSubmitDriveReport = false;
                    $scope.purposeErrorMessage = "* Du skal angive et formål.";
                }
            }


            angular.forEach($scope.KmRate, function (rate, key) {
                if ($scope.DriveReport.KmRate == rate.Id) {
                    if (rate.Type.RequiresLicensePlate && $scope.DriveReport.LicensePlate == "Ingen nummerplade") {
                        $scope.licensePlateErrorMessage = "* Det valgte transportmiddel kræver en nummerplade";
                        $scope.canSubmitDriveReport = false;
                    }
                }
            });

            if ($scope.guiChangedByMap <= 0) {
                $scope.generateMapWidget();
            }
            $scope.guiChangedByMap--;


        }



        $scope.Save = function () {
            $scope.validateInput();

            if (!$scope.canSubmitDriveReport) {
                return;
            }

            $scope.canSubmitDriveReport = false;

            var driveReport = new DriveReport();

            // Prepare all data to  be uploaded
            driveReport.Purpose = $scope.DriveReport.Purpose;
            driveReport.DriveDateTimestamp = Math.floor($scope.DriveReport.Date.getTime() / 1000);
            driveReport.KmRate = parseFloat(getKmRate().KmRate);
            driveReport.TFCode = getKmRate().Type.TFCode;

            driveReport.KilometerAllowance = $scope.DriveReport.KilometerAllowance;
            driveReport.Distance = 0;
            driveReport.AmountToReimburse = 0;

            if ($scope.showLicensePlate) {
                driveReport.LicensePlate = $scope.DriveReport.LicensePlate;
            } else {
                driveReport.LicensePlate = "0000000";
            }


            driveReport.PersonId = Person.Id;
            driveReport.FullName = Person.FullName;
            driveReport.Status = "Pending";
            driveReport.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
            driveReport.EditedDateTimestamp = driveReport.CreatedDateTimestamp;
            driveReport.Comment = "";
            driveReport.ClosedDateTimestamp = 0;
            driveReport.ProcessedDateTimestamp = 0;
            driveReport.EmploymentId = $scope.DriveReport.Position;

            if ($scope.DriveReport.KilometerAllowance === "Read") {

                driveReport.Distance = Number($scope.DriveReport.ReadDistance);
                driveReport.UserComment = $scope.DriveReport.UserComment;

                if ($scope.DriveReport.StartOrEndedAtHome === 'Started') {
                    driveReport.StartsAtHome = true;
                    driveReport.EndsAtHome = false;
                } else if ($scope.DriveReport.StartOrEndedAtHome === 'Ended') {
                    driveReport.StartsAtHome = false;
                    driveReport.EndsAtHome = true;
                } else if ($scope.DriveReport.StartOrEndedAtHome === 'Both') {
                    driveReport.StartsAtHome = true;
                    driveReport.EndsAtHome = true;
                } else {
                    driveReport.StartsAtHome = false;
                    driveReport.EndsAtHome = false;
                }
            } else {

                driveReport.StartsAtHome = false;
                driveReport.EndsAtHome = false;

                driveReport.DriveReportPoints = [];

                angular.forEach($scope.DriveReport.Addresses, function (address, key) {


                    var tempAddress = (address.Name.length != 0) ? address.Name : address.Personal;

                    var currentAddress = AddressFormatter.fn(tempAddress);

                    driveReport.DriveReportPoints.push({
                        StreetName: currentAddress.StreetName,
                        StreetNumber: currentAddress.StreetNumber,
                        ZipCode: currentAddress.ZipCode,
                        Town: currentAddress.Town,
                        Description: "",
                        Latitude: "",
                        Longitude: ""
                    });

                });

                if (typeof $scope.DriveReport.RoundTrip !== "undefined" && $scope.DriveReport.RoundTrip === true) {
                    for (var i = driveReport.DriveReportPoints.length - 2; i >= 0; --i) {
                        driveReport.DriveReportPoints.push(driveReport.DriveReportPoints[i]);
                    }
                }

                // go through addresses and see which is going to be saved
                angular.forEach($scope.DriveReport.Addresses, function (address, key) {

                    if (address.Save) {
                        var personalAddress = new PersonalAddress(AddressFormatter.fn(address.Name));

                        personalAddress.PersonId = Person.Id;
                        personalAddress.Type = PersonalAddressType.Standard;
                        personalAddress.Longitude = "";
                        personalAddress.Latitude = "";
                        personalAddress.Description = "";

                        delete personalAddress.Id;

                        personalAddress.$save();
                    }
                });
            }

            if (typeof $scope.DriveReport.FourKmRule !== "undefined" && $scope.DriveReport.FourKmRule.Using === true) {
                driveReport.FourKmRule = true;
            } else {
                driveReport.FourKmRule = false;
            }

            driveReport.$save(function (response) {
                LatestDriveReport = response;
                $scope.TransportAllowance = 0;
                $scope.RemainingKilometers = 0;
                $scope.PayoutAmount = response.AmountToReimburse.toFixed(2).toString().replace('.', ',');
                NotificationService.AutoFadeNotification("success", "", "Din tjenestekørselsindberetning blev gemt");
                $scope.clearClicked();

            }, function (response) {
                // failure
                NotificationService.AutoFadeNotification("danger", "", "Din tjenestekørselsindberetning blev ikke gemt");
            });
        };

        $scope.AddViapoint = function () {
            var temp = $scope.DriveReport.Addresses.pop();
            $scope.DriveReport.Addresses.push({ Name: "", Personal: "", Save: false });
            $scope.DriveReport.Addresses.push(temp);
        };

        $scope.Remove = function (array, index) {
            array.splice(index, 1);
            $scope.addressInputChanged(index);
        };

        $scope.clearClicked = function () {
            // Make the datepicker pop open when clear is clicked.
            openDatePicker = true;
            $scope.DriveReport.Purpose = "";
            $scope.container.PersonalRouteDropDown.select(0);
            $scope.container.PersonalRouteDropDown.trigger("change");
            $scope.container.PersonalAddressDropDown.select(0);
            $scope.container.PersonalAddressDropDown.trigger("change");
            $scope.DriveReport.FourKmRule = {};
            $scope.DriveReport.FourKmRule.Using = false;
            $scope.DriveReport.FourKmRule.Value = "";
            $scope.DriveReport.RoundTrip = false;
            $scope.DriveReport.ReadDistance = "";
            $scope.DriveReport.UserComment = "";

            $scope.loadValuesFromLatestDriveReport();

            // If you dont do this, then the change function will be called and the address fields will be filled with the default addresses.
            $scope.mapChangedByGui = true;

            OS2RouteMap.set(MapStartAddress);
        }

        $scope.loadValuesFromLatestDriveReport = function () {
            $scope.PositionDropDown.select(function (item) {
                return item.Id === LatestDriveReport.EmploymentId;
            });

            $scope.LicensePlateDropDown.select(function (item) {

                return item.Plate == LatestDriveReport.LicensePlate;
            });

            $scope.container.KmRateDropDown.select(function (item) {
                return item.Type.TFCode == LatestDriveReport.TFCode;
            });

            $scope.container.KmRateDropDown.trigger("change");

            $scope.container.kilometerAllowanceDropDown.select(function (item) {
                return item.value == LatestDriveReport.KilometerAllowance;
            });

            $scope.kilometerAllowanceChanged();
        };

        $scope.kilometerAllowanceChanged = function () {
            $scope.DriveReport.KilometerAllowance = $scope.container.kilometerAllowanceDropDown._selectedValue;
            $scope.container.kilometerAllowanceDropDown.trigger("change");
            if ($scope.DriveReport.KilometerAllowance != "Read") {
                $scope.DriveReport.UserComment = "";
                $scope.DriveReport.ReadDistance = 0;
            }
        }

        var openDatePicker = true;
        // Open the datepicker when the page finishes loading
        $scope.$on("kendoRendered", function (event) {
            if (openDatePicker) {
                $scope.driveDatePicker.open();
                openDatePicker = false;
                $scope.loadValuesFromLatestDriveReport();
            }
            $scope.validateInput();

        });

        $scope.generateMapWidget = function () {

            var setCheckArrayIndexAndPopulateMap = function (key, checkArray) {
                checkArray[key] = true;
                if (checkArray.every(function (element, index, array) {
                                    return element;
                })) {
                    $scope.populateMap();
                }
            }


            var checkArray = [];
            angular.forEach($scope.DriveReport.Addresses, function (value, key) {
                checkArray[key] = false;
            });

            angular.forEach($scope.DriveReport.Addresses, function (address, key) {
                checkArray[key] = false;

                if ($scope.isAddressNameSet(address) === false && (address.Personal == "" || address.Personal == "Vælg fast adresse" || address.Personal == undefined)) {
                    // Data is not valid.
                    return;
                } else if ($scope.isAddressNameSet(address) === true) {
                    var format = AddressFormatter.fn(address.Name);

                    if (format != undefined) {
                        if (address.Latitude == undefined) {
                            Address.setCoordinatesOnAddress({ StreetName: format.StreetName, StreetNumber: format.StreetNumber, ZipCode: format.ZipCode, Town: format.Town }, function (res) {
                                address.Latitude = res[0].Latitude;
                                address.Longitude = res[0].Longitude;

                                setCheckArrayIndexAndPopulateMap(key, checkArray);


                            });
                        } else {
                            setCheckArrayIndexAndPopulateMap(key, checkArray);
                        }
                    }
                } else {
                    var format = AddressFormatter.fn(address.Personal);
                    if (format != undefined) {
                        if (address.Latitude == undefined) {
                            Address.setCoordinatesOnAddress({ StreetName: format.StreetName, StreetNumber: format.StreetNumber, ZipCode: format.ZipCode, Town: format.Town }, function (res) {
                                address.Latitude = res[0].Latitude;
                                address.Longitude = res[0].Longitude;

                                setCheckArrayIndexAndPopulateMap(key, checkArray);

                            });
                        } else {
                            setCheckArrayIndexAndPopulateMap(key, checkArray);
                        }
                    }
                }



            });
        }

        $scope.populateMap = function () {
            var mapArray = [];

            angular.forEach($scope.DriveReport.Addresses, function (address, key) {
                var name = (function () {
                    if ($scope.isAddressNameSet(address) === false) {
                        return address.Personal;
                    }
                    return address.Name;
                })();

                mapArray.push({ name: name, lat: address.Latitude, lng: address.Longitude });
            });

            $scope.mapChangedByGui = true;
            OS2RouteMap.set(mapArray);

        }

        $scope.isAddressNameSet = function (address) {
            return !(address.Name == "" || address.Name == $scope.addressPlaceholderText || address.Name == undefined);
        }

        var routeMapChanged = function (obj) {

            updateDrivenKilometerFields(obj);

            if (!$scope.mapChangedByGui) {
                // Clear personal route dropdown.
                $scope.isRoute = false;
                $scope.container.PersonalRouteDropDown.select(0);
                $scope.container.PersonalRouteDropDown.trigger("change");

                // Empty the addresses in the current driveReport
                $scope.DriveReport.Addresses = [];
                // Iterate all selected addresses on the map and push them to the drivereport
                angular.forEach(obj.Addresses, function (address, key) {
                    var shavedName = $scope.shaveExtraCommasOffAddressString(address.name);
                    $scope.DriveReport.Addresses.push({ Personal: "", Name: shavedName, Latitude: address.lat, Longitude: address.lng });
                });
                $scope.guiChangedByMap = obj.Addresses.length;



                // apply to notify angular and have it run ng-repeat, filling in the addreses in the view.
                $scope.$apply();
            }
            $scope.mapChangedByGui = false;

        }

        $scope.shaveExtraCommasOffAddressString = function (address) {
            var res = address.replace(/,/, "###");
            res = res.replace(/,/g, "");
            res = res.replace(/###/, ",");
            return res;
        }


        $scope.addressInputChanged = function (index) {
            if ($scope.guiChangedByMap <= 0) {
                $scope.DriveReport.Addresses[index].Latitude = undefined;
                $scope.DriveReport.Addresses[index].Longitude = undefined;
            }
            $scope.validateInput();
        }

        $scope.employmentChanged = function () {
            // Clear the checkbox and the value field before checking.
            $scope.DriveReport.FourKmRule = {};
            $scope.DriveReport.FourKmRule.Using = false;
            $scope.DriveReport.FourKmRule.Value = "";

            // Is there a better way to do this?
            // My guess is this might take a long time if there are a lot of org units. 
            angular.forEach(PersonEmployments, function (empl, key) {
                // Show checkbox and value field, if the chosen orgunit allows it. 
                if (empl.Id == $scope.DriveReport.Position) {
                    $scope.hasAccessToFourKmRule = empl.OrgUnit.HasAccessToFourKmRule;
                }
            });
        }

        $scope.readDistanceChanged = function () {
            updateDrivenKilometerFields();
        }

        var updateDrivenKilometerFields = function (obj) {
            if ($scope.DriveReport.KilometerAllowance === "Read") {
                $scope.DrivenKilometers = $scope.DriveReport.ReadDistance;
            } else {
                $scope.DrivenKilometers = obj.distance.toFixed(2).toString().replace('.', ',');
                $scope.$apply();
            }
        }

        $scope.$watch("DrivenKilometers", function () {
            $scope.drivenKmChanged();
        });

        $scope.drivenKmChanged = function () {
            if ($scope.DriveReport.KilometerAllowance == "CalculatedWithoutExtraDistance") {
                $scope.TransportAllowance = 0;
            } else if ($scope.DriveReport.KilometerAllowance == "Calculated") {
                var toSubtract = 0;
                if ($scope.DriveReport.Addresses[0].Name.indexOf(HomeAddress) > -1 || $scope.DriveReport.Addresses[0].Personal.indexOf(HomeAddress) > -1) {
                    toSubtract += Person.DistanceFromHomeToWork;
                }
                if ($scope.DriveReport.Addresses[$scope.DriveReport.Addresses.length - 1].Name.indexOf(HomeAddress) > -1 || $scope.DriveReport.Addresses[$scope.DriveReport.Addresses.length - 1].Personal.indexOf(HomeAddress) > -1) {
                    toSubtract += Person.DistanceFromHomeToWork;
                }
                $scope.TransportAllowance = toSubtract;
            } else {
                if ($scope.DriveReport.StartOrEndedAtHome == "Both") {
                    $scope.TransportAllowance = 2 * Number(Person.DistanceFromHomeToWork);
                } else if ($scope.DriveReport.StartOrEndedAtHome == "Neither") {
                    $scope.TransportAllowance = 0;
                } else {
                    $scope.TransportAllowance = Person.DistanceFromHomeToWork;
                }
            }

            $scope.TransportAllowance = Number($scope.TransportAllowance.toString().replace(",", ".")).toFixed(2).toString().replace(".", ",");


            if ($scope.DriveReport.RoundTrip === true) {
                $scope.DrivenKMDisplay = (2 * Number($scope.DrivenKilometers.toString().replace(",", "."))).toString().replace(".", ",");
            } else {
                $scope.DrivenKMDisplay = $scope.DrivenKilometers.toString().replace(".", ",");
            }
            var remKM = Number($scope.DrivenKMDisplay.toString().replace(",", ".")) - Number($scope.TransportAllowance.toString().replace(",", "."));
            if (remKM > 0) {
                $scope.RemainingKilometers = Number(remKM).toFixed(2).toString().replace(".", ",");
            } else {
                $scope.RemainingKilometers = 0;
            }

        }

        $scope.roundTripChanged = function () {
            $scope.drivenKmChanged();
        }

        OS2RouteMap.create({
            id: 'map',
            change: routeMapChanged
        });

        OS2RouteMap.set(MapStartAddress);
    }
]);