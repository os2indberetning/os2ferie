angular.module("application").controller("EditReportController", [
    "$scope", "SmartAdresseSource", "DriveReport", "PersonalAddress", "AddressFormatter", "PersonalAddressType", "Person", "PersonEmployments", "Rate", "LicensePlate", "NotificationService", "$modal", "$state", "Address", "Route", "reportId", function ($scope, SmartAdresseSource, DriveReport, PersonalAddress, AddressFormatter, PersonalAddressType, Person, PersonEmployments, Rate, LicensePlate, NotificationService, $modal, $state, Address, Route, reportId) {

        var personId = 1;

        $scope.container = {};

        // Is filled with the default address for the map widget.
        var mapStartAddress = [];

        // Magic variable. Is checked when calling generateMapWidget to make sure it is only called when we manually change the gui. IE. not by changes on the map.
        // When the map is changes by the map, the variable is set to the number of address points and is decremented by one for each time a gui element changes
        // which it does once for each address.
        // Simply put: if the var is <= 0 then the map will be drawn.
        $scope.guiChangedByMap = 0;

        $scope.DriveReport = DriveReport.getWithPoints({ id: reportId }, function () {
            $scope.DriveReport.Date = moment.unix($scope.DriveReport.DriveDateTimestamp)._d;
            $scope.DriveReport.Addresses = [];

            angular.forEach($scope.DriveReport.DriveReportPoints, function (point, key) {
                $scope.DriveReport.Addresses.push({ Name: point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town, Personal: "", Save: false });
            });



            var temp4Km = $scope.DriveReport.FourKmRule;
            $scope.DriveReport.FourKmRule = {};
            $scope.DriveReport.FourKmRule.Using = temp4Km;
        });





        $scope.canSubmitDriveReport = true;
        $scope.Routes = [];
        $scope.IsRoute = false;



        $scope.personalRouteDropdownChange = function (e) {
            var index = e.sender.selectedIndex;

            if (index == 0) {
                $scope.IsRoute = false;
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

        }

        $scope.validateInput = function () {
            $scope.canSubmitDriveReport = true;
            $scope.purposeErrorMessage = "";
            $scope.readDistanceErrorMessage = "";
            $scope.addressSelectionErrorMessage = "";
            $scope.userCommentErrorMessage = "";
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
                    if (address.Name == "" && address.Personal == "Vælg fast adresse") {
                        $scope.canSubmitDriveReport = false;
                        $scope.addressSelectionErrorMessage = "* Du skal udfylde alle adressefelter.";
                    }
                });
                if ($scope.DriveReport.Purpose == "" || $scope.DriveReport.Purpose == undefined) {
                    $scope.canSubmitDriveReport = false;
                    $scope.purposeErrorMessage = "* Du skal angive et formål.";
                }
            }
            if ($scope.guiChangedByMap <= 0) {
                $scope.generateMapWidget();
            }
            $scope.guiChangedByMap--;


        }

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




            // Set mapChangedByGui to true.
            // If you dont do this, then the change function will be called and the address fields will be filled with the default addresses.
            $scope.mapChangedByGui = true;

            OS2RouteMap.set($scope.getDefaultMapAddresses());



        }

        $scope.kilometerAllowanceChanged = function () {
            $scope.DriveReport.KilometerAllowance = $scope.container.kilometerAllowanceDropDown._selectedValue;
            if ($scope.DriveReport.KilometerAllowance != "Read") {
                $scope.DriveReport.UserComment = "";
                $scope.DriveReport.ReadDistance = 0;
            }
        }


        $scope.$on("kendoRendered", function (event) {
            $scope.KmRate.$promise.then(function () {
                $scope.DriveReport.$promise.then(function () {
                    $scope.KmRateDropDown.dataSource.read();
                    $scope.KmRateDropDown.select(function (item) {
                        return item.Type.TFCode == $scope.DriveReport.TFCode;
                    });
                });

            });

            $scope.Employments.$promise.then(function () {
                $scope.PositionDropDown.dataSource.read();

            });

            $scope.LicensePlates.$promise.then(function (data) {
                $scope.LicensePlateDropDown.dataSource.read();
                $scope.canSubmitDriveReport = data.length > 0;
            });


            $scope.validateInput();

        });

        $scope.Employments = PersonEmployments.get({ id: personId });

        $scope.LicensePlates = LicensePlate.get({ id: personId });

        $scope.KmRate = Rate.ThisYearsRates();

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
            OS2RouteMap.set(mapArray);

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

                var drivenKm = Number($scope.DriveReport.ReadDistance.toString().replace(',', '.'));

                var remKm = Number(drivenKm - $scope.Person.DistanceFromHomeToWork);

                $scope.RemainingKilometers = remKm.toFixed(2).toString().replace('.', ',');
                if (remKm < 0) {
                    $scope.RemainingKilometers = 0;
                }
            } else {
                var remKm = Number(obj.distance - $scope.Person.DistanceFromHomeToWork);
                $scope.RemainingKilometers = remKm.toFixed(2).toString().replace('.', ',');
                if (remKm < 0) {
                    $scope.RemainingKilometers = 0;
                }
                $scope.DrivenKilometers = obj.distance.toFixed(2).toString().replace('.', ',');
            }
            $scope.$apply();
        }

        $scope.saveEditClick = function () {
            var driveReport = new DriveReport();

            if ($scope.showLicensePlate) {
                driveReport.LicensePlate = $scope.DriveReport.LicensePlate;
            } else {
                driveReport.LicensePlate = "0000000";
            }

            driveReport.Distance = 0;
            driveReport.UserComment = "";

            if ($scope.DriveReport.KilometerAllowance === "Read") {

                driveReport.Distance = Number($scope.DriveReport.ReadDistance) * 1000;
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

                driveReport.FourKmRule = $scope.DriveReport.FourKmRule.Using;

            }

            debugger;

            DriveReport.patch({ id: $scope.DriveReport.Id }, {
                Purpose: $scope.DriveReport.Purpose,
                DriveDateTimestamp: Math.floor($scope.DriveReport.Date.getTime() / 1000),
                KmRate: parseFloat(getKmRate().KmRate),
                TFCode: getKmRate().Type.TFCode,
                KilometerAllowance: $scope.DriveReport.KilometerAllowance,
                LicensePlate: driveReport.LicensePlate,
                EditedDateTimestamp: Math.floor(Date.now() / 1000),
                EmploymentId: $scope.DriveReport.Position,
                StartsAtHome: driveReport.StartsAtHome,
                EndsAtHome: driveReport.EndsAtHome,
                DriveReportPoints: [],
                FourKmRule: driveReport.FourKmRule,
                Distance: driveReport.Distance,
                UserComment: driveReport.UserComment,
            });
        }

        $scope.cancelClick = function () {

        }

    }
]);