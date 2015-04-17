angular.module("application").controller("DrivingController", [
    "$scope", "SmartAdresseSource", "DriveReport", "PersonalAddress", "AddressFormatter", "PersonalAddressType", "Person", "PersonEmployments", "Rate", "LicensePlate", "NotificationService", "$modal", "$state", "Address", "Route", "$q", "HelpText", function ($scope, SmartAdresseSource, DriveReport, PersonalAddress, AddressFormatter, PersonalAddressType, Person, PersonEmployments, Rate, LicensePlate, NotificationService, $modal, $state, Address, Route, $q, HelpText) {

        $scope.DrivenKMDisplay = 0;

        $scope.container = {};

        $scope.addressPlaceholderText = 'Eller skriv adresse her';

        $scope.ReadReportCommentHelp = HelpText.get({ id: "ReadReportCommentHelp" });

        // Hardcoded personId
        var personId = 1;

        // Is filled with the default address for the map widget.
        var mapStartAddress = [];

        // Magic variable. Is checked when calling generateMapWidget to make sure it is only called when we manually change the gui. IE. not by changes on the map.
        // When the map is changes by the map, the variable is set to the number of address points and is decremented by one for each time a gui element changes
        // which it does once for each address.
        // Simply put: if the var is <= 0 then the map will be drawn.
        $scope.guiChangedByMap = 0;

        $scope.DriveReport = new DriveReport();
        $scope.canSubmitDriveReport = true;
        $scope.Routes = [];
        $scope.IsRoute = false;

        $scope.personalRouteDropdownChange = function (e) {
            var index = e.sender.selectedIndex;

            if (index == 0) {
                $scope.IsRoute = false;
                $scope.DriveReport.Addresses = [];

                $scope.DriveReport.Addresses.push({ Name: "", Save: false });
                $scope.DriveReport.Addresses.push({ Name: "", Save: false });
                return;
            }

            $scope.IsRoute = true;

            var route = $scope.Routes[index - 1];

            var lastIndex = route.Points.length - 1;

            $scope.DriveReport.Addresses = [];

            angular.forEach($scope.Routes[index - 1].Points, function (value, key) {
                $scope.DriveReport.Addresses.push({ Name: value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town, Save: false });
            });

            $scope.validateInput();
        }


        $scope.getDefaultMapAddresses = function () {
            return mapStartAddress;
        }

        $scope.Person = Person.get({ id: personId }, function () {
            // Show the persons distance from home to work.
            $scope.TransportAllowance = $scope.Person.DistanceFromHomeToWork.toFixed(2).toString().replace('.', ',');

            Address.GetPersonalAndStandard({ personId: personId }, function (data) {
                var temp = [{ value: "Vælg fast adresse" }];
                angular.forEach(data, function (value, key) {
                    var street = value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;
                    var presentation = (function () {
                        if (value.Type == "Home") {
                            value.Description = "Hjemmeadresse";
                        } else if (value.Type == "Work") {
                            value.Description = "Arbejdsadresse";
                        } else if (value.Type == "AlternativeHome") {
                            value.Description = "Afvigende hjemmeadresse";
                        } else if (value.Type == "AlternativeWork") {
                            value.Description = "Afvigende arbejdsadresse";
                        }
                        if (value.Description != "" && value.Description != undefined) {
                            return value.Description + " : " + street;
                        }
                        return street;
                    })();
                    temp.push({ value: presentation, StreetName: street });
                });

                $scope.PersonalAddresses = temp;
            });

            Route.get({ query: "&filter=PersonId eq " + personId }, function (data) {

                var temp = [{ addressOne: "", addressTwo: "", viaPointCounr: "", presentation: "" }];

                angular.forEach(data.value, function (value, key) {
                    var one = value.Points[0].StreetName + " " + value.Points[0].StreetNumber + ", " + value.Points[0].ZipCode + " " + value.Points[0].Town;
                    var two = value.Points[value.Points.length - 1].StreetName + " " + value.Points[value.Points.length - 1].StreetNumber + ", " + value.Points[value.Points.length - 1].ZipCode + " " + value.Points[value.Points.length - 1].Town;
                    var count = value.Points.length - 2;
                    temp.push({ addressOne: one, addressTwo: two, viaPointCount: count, presentation: value.Description + ": " + one + " -> " + two + " | Antal viapunkter: " + count, routeId: value.Id });
                    $scope.Routes.push(value);
                });

                $scope.PersonalRoutes = temp;
            });
        });

        $scope.FourKmRule = {}
        $scope.FourKmRule.Using = false;

        $scope.DriveReport.Addresses = [];

        $scope.DriveReport.Addresses.push({ Name: "", Personal: "", Save: false });
        $scope.DriveReport.Addresses.push({ Name: "", Personal: "", Save: false });

        $scope.SmartAddress = SmartAdresseSource;

        $scope.DriveReport.Date = new Date();

        $scope.DateOptions = {
            start: "month"
        };

        $scope.DrivenKilometers = 0;
        $scope.TransportAllowance = 0;
        $scope.RemainingKilometers = 0;
        $scope.PayoutAmount = 0;

        var getKmRate = function () {
            for (var i = 0; i < $scope.KmRate.length; i++) {
                if ($scope.KmRate[i].Id == $scope.DriveReport.KmRate) {
                    return $scope.KmRate[i];
                }
            }
        }

        $scope.transportChanged = function () {
            $scope.lastSelectedTransport = $scope.KmRateDropDown.select();
            $scope.showLicensePlate = true;
            angular.forEach($scope.KmRate, function (rate, key) {
                if ($scope.DriveReport.KmRate == rate.Id) {
                    $scope.showLicensePlate = rate.Type.RequiresLicensePlate;
                }
            });
            $scope.validateInput();

        }

        $scope.validateInput = function () {
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
                    console.log("from validate");
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


            driveReport.PersonId = $scope.Person.Id;
            driveReport.FullName = $scope.Person.FullName;
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

                    var currentAddress = new PersonalAddress(AddressFormatter.fn(tempAddress));

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

                        personalAddress.PersonId = $scope.Person.Id;
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
                // success

                latestDriveReport = response;

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

        $scope.openNoLicensePlateModal = function () {

            var modalInstance = $modal.open({
                templateUrl: '/App/Driving/noLicensePlateModal.html',
                controller: 'noLicensePlateModalController',
                //size: size,
                backdrop: 'static',
                resolve: {

                }
            });

            modalInstance.result.then(function () {
                $state.go("settings");
            }, function () {

            });
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




            // Set mapChangedByGui to true.
            // If you dont do this, then the change function will be called and the address fields will be filled with the default addresses.
            $scope.mapChangedByGui = true;

            OS2RouteMap.set($scope.getDefaultMapAddresses());



        }

        $scope.loadValuesFromLatestDriveReport = function () {

            if (latestDriveReport.$promise == undefined) {
                // This if is true, if latestDriveReport is the result of a post.
                $scope.PositionDropDown.select(function (item) {
                    return item.Id === latestDriveReport.EmploymentId;
                });

                $scope.LicensePlateDropDown.select(function (item) {
                    return item.Plate == latestDriveReport.LicensePlate;

                });

                $scope.KmRateDropDown.select(function (item) {
                    return item.Type.TFCode == latestDriveReport.TFCode;
                });

                $scope.container.kilometerAllowanceDropDown.select(function (item) {
                    return item.value == latestDriveReport.KilometerAllowance;
                });

                $scope.kilometerAllowanceChanged();
            } else {
                // Else will be hit when the page is freshly loaded and latestDriveReport is retrieved from the server via get.
                latestDriveReport.$promise.then(function (res) {

                    $scope.PositionDropDown.select(function (item) {
                        return item.Id === res.Employment.Id;
                    });

                    $scope.LicensePlateDropDown.select(function (item) {
                        return item.Plate == res.LicensePlate;
                    });

                    $scope.KmRateDropDown.select(function (item) {
                        return item.Type.TFCode == res.TFCode;
                    });

                    $scope.container.kilometerAllowanceDropDown.select(function (item) {
                        return item.value == res.KilometerAllowance;
                    });

                    $scope.kilometerAllowanceChanged();
                });
            }
        }

        $scope.kilometerAllowanceChanged = function () {
            $scope.DriveReport.KilometerAllowance = $scope.container.kilometerAllowanceDropDown._selectedValue;
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

            $scope.KmRate.$promise.then(function () {
                $scope.KmRateDropDown.dataSource.read();
                $scope.KmRateDropDown.select($scope.lastSelectedTransport);
            });

            $scope.Employments.$promise.then(function (data) {
                angular.forEach(data, function (employment, key) {
                    employment.PresentationString = employment.Position + " - " + employment.OrgUnit.ShortDescription;
                });
                $scope.PositionDropDown.dataSource.read();

            });

            $scope.LicensePlates.$promise.then(function (data) {
                if ($scope.LicensePlates.length > 0) {
                    angular.forEach(data, function (plate, key) {
                        plate.PresentationString = plate.Plate + " - " + plate.Description;
                    });
                    $scope.LicensePlateDropDown.dataSource.read();
                    $scope.canSubmitDriveReport = data.length > 0;
                } else {
                    $scope.LicensePlates = [{ PresentationString: "Ingen nummerplade" }];
                }
            });


            $scope.validateInput();

        });

        $scope.Employments = PersonEmployments.get({ id: personId });

        $scope.LicensePlates = LicensePlate.get({ id: personId });


        $scope.KmRate = Rate.ThisYearsRates();

        var latestDriveReport = DriveReport.getLatest({ id: personId });


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

                console.log("From generate map widget");
                if ($scope.isAddressNameSet(address) === false && (address.Personal == "" || address.Personal == "Vælg fast adresse" || address.Personal == undefined)) {
                    // Data is not valid.
                    return;
                } else if (address.Name != "") {
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
                    console.log("From populate map");
                    if ($scope.isAddressNameSet(address) === false ) {
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
            console.log("Checking if address name is set for " + address);
            return !( address.Name == "" || address.Name == $scope.addressPlaceholderText || address.Name == undefined);
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
                    $scope.DriveReport.Addresses.push({ Name: shavedName, Latitude: address.lat, Longitude: address.lng });
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

        Address.getMapStart(function (res) {
            mapStartAddress = [
                { name: res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town, lat: res.Latitude, lng: res.Longitude },
                { name: res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town, lat: res.Latitude, lng: res.Longitude }
            ];

            OS2RouteMap.create({
                id: 'map',
                change: routeMapChanged
            });

            OS2RouteMap.set($scope.getDefaultMapAddresses());
        });



        $scope.employmentChanged = function () {
            // Clear the checkbox and the value field before checking.
            $scope.DriveReport.FourKmRule = {};
            $scope.DriveReport.FourKmRule.Using = false;
            $scope.DriveReport.FourKmRule.Value = "";

            // Is there a better way to do this?
            // My guess is this might take a long time if there are a lot of org units. 
            angular.forEach($scope.Employments, function (empl, key) {
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
            }
        }

        $scope.$watch("DrivenKilometers", function () {
            drivenKmChanged();
        });

        var drivenKmChanged = function () {
            if ($scope.DriveReport.RoundTrip === true) {
                $scope.DrivenKMDisplay = (2 * Number($scope.DrivenKilometers.toString().replace(",", "."))).toString().replace(".", ",");
            } else {
                $scope.DrivenKMDisplay = $scope.DrivenKilometers.toString().replace(".", ",");
            }
            var remKM = Number($scope.DrivenKMDisplay.toString().replace(",", ".")) - Number($scope.Person.DistanceFromHomeToWork);
            if (remKM > 0) {
                $scope.RemainingKilometers = Number(remKM).toFixed(2).toString().replace(".", ",");
            } else {
                $scope.RemainingKilometers = 0;
            }
        }

        $scope.roundTripChanged = function () {
            drivenKmChanged();
        }
    }
]);