angular.module("application").controller("DrivingController", [
    "$scope", "SmartAdresseSource", "DriveReport", "PersonalAddress", "AddressFormatter", "PersonalAddressType", "Person", "PersonEmployments", "Rate", "LicensePlate", "NotificationService", "$modal", "$state", "Address", "Route", "$q", function ($scope, SmartAdresseSource, DriveReport, PersonalAddress, AddressFormatter, PersonalAddressType, Person, PersonEmployments, Rate, LicensePlate, NotificationService, $modal, $state, Address, Route, $q) {


        $scope.container = {};


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

        $scope.Person = Person.get({ id: 1 }, function () {
            Address.get({ query: "$filter=PersonId eq " + $scope.Person.Id + " and Type eq Core.DomainModel.PersonalAddressType'Standard'" }, function (data) {
                var temp = [{ value: "Vælg fast adresse" }];

                angular.forEach(data.value, function (value, key) {
                    var street = value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;
                    var presentation = (function () {
                        if (value.Description != "" && value.Description != undefined) {
                            return value.Description + " : " + street;
                        }
                        return street;
                    })();
                    temp.push({ value: presentation, StreetName: street });
                });

                $scope.PersonalAddresses = temp;
            });

            Route.get({ query: "&filter=PersonId eq " + 1 }, function (data) {

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

        $scope.validateInput = function () {
            $scope.canSubmitDriveReport = true;

            if ($scope.DriveReport.KilometerAllowance === "Read") {
                if ($scope.DriveReport.Purpose == "" || $scope.DriveReport.Purpose == undefined) {
                    $scope.canSubmitDriveReport = false;
                }
                if ($scope.DriveReport.ReadDistance === "" || $scope.DriveReport.ReadDistance == undefined) {
                    $scope.canSubmitDriveReport = false;
                }
            } else {
                angular.forEach($scope.DriveReport.Addresses, function (address, key) {
                    if (address.Name == "" && address.Personal == "Vælg fast adresse") {
                        $scope.canSubmitDriveReport = false;
                    }
                });
                if ($scope.DriveReport.Purpose == "" || $scope.DriveReport.Purpose == undefined) {
                    $scope.canSubmitDriveReport = false;
                }
            }
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

            var driveReport = new DriveReport();

            // Prepare all data to  be uploaded
            driveReport.Purpose = $scope.DriveReport.Purpose;
            driveReport.DriveDateTimestamp = Math.floor($scope.DriveReport.Date.getTime() / 1000);
            driveReport.KmRate = parseFloat(getKmRate().KmRate);
            driveReport.TFCode = getKmRate().Type.TFCode;

            driveReport.KilometerAllowance = $scope.DriveReport.KilometerAllowance;
            driveReport.Distance = 0;
            driveReport.AmountToReimburse = 0;
            driveReport.Licenseplate = $scope.DriveReport.Licenseplate;
            driveReport.PersonId = $scope.Person.Id;
            driveReport.Status = "Pending";
            driveReport.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
            driveReport.EditedDateTimestamp = driveReport.CreatedDateTimestamp;
            driveReport.Comment = "";
            driveReport.ClosedDateTimestamp = 0;
            driveReport.ProcessedDateTimestamp = 0;
            driveReport.EmploymentId = $scope.DriveReport.Position;

            // These two should be removed, they are in a future branch
            //driveReport.Fullname = "";
            //driveReport.Timestamp = "";

            if ($scope.DriveReport.KilometerAllowance === "Read") {

                driveReport.Distance = Number($scope.DriveReport.ReadDistance) * 1000;

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
                    for (var i = driveReport.DriveReportPoints.length - 1; i > 0; --i) {
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
                $scope.DrivenKilometers = response.Distance.toFixed(2).toString().replace('.',',');
                $scope.TransportAllowance = 0;
                $scope.RemainingKilometers = 0;
                $scope.PayoutAmount = response.AmountToReimburse.toFixed(2).toString().replace('.',',');
                NotificationService.AutoFadeNotification("success", "Success", "Din kørselsindberetning blev gemt");


            }, function (response) {
                // failure
                NotificationService.AutoFadeNotification("danger", "Fejl", "Din kørselsindberetning blev ikke gemt");
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


        }


        var openDatePicker = true;
        // Open the datepicker when the page finishes loading
        $scope.$on("kendoRendered", function (event) {
            if (openDatePicker) {
                $scope.driveDatePicker.open();
                openDatePicker = false;
            }

            // Load data into kendo components once they are finished rendering.
            $scope.KmRate = Rate.ThisYearsRates(function () {
                $scope.KmRateDropDown.dataSource.read();
            });

            $scope.Employments = PersonEmployments.get({ id: 1 }, function () {
                $scope.PositionDropDown.dataSource.read();
            });

            $scope.Licenseplates = LicensePlate.get({ id: 1 }, function (data) {
                if (data.length > 0) {
                    $scope.LicenseplateDropDown.dataSource.read();
                    $scope.canSubmitDriveReport = data.length > 0;
                } else {
                    $scope.openNoLicensePlateModal();
                }
            });




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

                if ((address.Name == "" || address.Name == undefined) && (address.Personal == "" || address.Personal == "Vælg fast adresse" || address.Personal == undefined)) {
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
                    if (address.Name == "") {
                        return address.Personal;
                    }
                    return address.Name;
                })();

                mapArray.push({ name: name, lat: address.Latitude, lng: address.Longitude });
            });

            $scope.mapChangedByGui = true;
            OS2RouteMap.show({
                id: 'map',
                Addresses: mapArray,
                change: routeMapChanged
            });
        }

        var routeMapChanged = function (obj) {
            $scope.DrivenKilometers = obj.distance.toFixed(2).toString().replace('.',',');
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


        $scope.addressInputChanged = function(index) {
            if ($scope.guiChangedByMap <= 0) {
                $scope.DriveReport.Addresses[index].Latitude = undefined;
                $scope.DriveReport.Addresses[index].Longitude = undefined;
            }
            $scope.validateInput();
        }

            // Clear the routemap fields.
            // If you dont do this, then the map wont load when navigating to a different page and back again.
            OS2RouteMap.id = null;
            OS2RouteMap.map = null;
            OS2RouteMap.options = null;
            OS2RouteMap.routeControl = null;

            OS2RouteMap.show({
                id: 'map',
                Addresses: [
                    { name: "Banegårdspladsen 1, 8000, Aarhus C", lat: 56.1504, lng: 10.2045 },
                    { name: "Banegårdspladsen 1, 8000, Aarhus C", lat: 56.1504, lng: 10.2045 }
                ],
                change: routeMapChanged
            });
    }
]);