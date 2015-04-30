angular.module("application").controller("DrivingController", [
    "$scope", "Person", "PersonEmployments", "Rate", "LicensePlate", "PersonalRoute", "DriveReport", "Address", "SmartAdresseSource", "AddressFormatter",
    function ($scope, Person, PersonEmployments, Rate, LicensePlate, PersonalRoute, DriveReport, Address, SmartAdresseSource, AddressFormatter) {

        $scope.DriveReport = new DriveReport();
        $scope.DriveReport.Addresses = [];
        $scope.DriveReport.Addresses.push({ Name: "", Personal: "" });
        $scope.DriveReport.Addresses.push({ Name: "", Personal: "" });
        $scope.addressPlaceholderText = "Eller indtast adresse her";
        $scope.addressDropDownPlaceholderText = "Vælg fast adresse";
        $scope.IsRoute = false;
        $scope.SmartAddress = SmartAdresseSource;

        // Set the datepicker date to today.
        $scope.DriveReport.Date = new Date();

        // Load all data
        Person.GetCurrentUser().$promise.then(function (currentUser) {
           
            // Load user's positions.
            PersonEmployments.get({ id: currentUser.Id }).$promise.then(function (res) {
                angular.forEach(res, function (value, key) {
                    value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription;
                });
                $scope.Employments = res;
            });

            // Load this year's rates.
            Rate.ThisYearsRates().$promise.then(function(res) {
                $scope.KmRate = res;
            });

            // Load user's license plates.
            LicensePlate.get({ id: currentUser.Id }).$promise.then(function (res) {
                angular.forEach(res, function (value, key) {
                    value.PresentationString = value.Plate + " - " + value.Description;
                });
                $scope.LicensePlates = res;
            });

            // Load user's personal routes
            PersonalRoute.getForUser({id: currentUser.Id }).$promise.then(function (res) {
                angular.forEach(res, function(value, key) {
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
            });

            Address.getMapStart().$promise.then(function (res) {
                OS2RouteMap.create({
                    id: 'map'
                });
                OS2RouteMap.set(res);
            });

            Address.GetPersonalAndStandard({ personId: currentUser.Id }).$promise.then(function(res) {
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

        $scope.personalRouteDropdownChange = function (e) {
            if (e.sender.selectedIndex == 0) {
                $scope.IsRoute = false;
            } else {
                $scope.IsRoute = true;
            }
        }

        $scope.isAddressNameSet = function (address) {
            return !(address.Name == "" || address.Name == $scope.addressPlaceholderText || address.Name == undefined);
        }

        $scope.addressInputChanged = function (e) {
            var promises = [];
            var mapArray = [];

            var addr = $scope.DriveReport.Addresses[e];
            if (!$scope.isAddressNameSet(addr) && addr.Personal != $scope.addressDropDownPlaceholderText) {
                var pers = getPersonalAddress(addr.Personal);
                mapArray.push({ lat: addr.Latitude, lng: addr.Longitude });
            } else if ($scope.isAddressNameSet(addr)) {
                var format = AddressFormatter.fn(addr.Name);
                Address.setCoordinatesOnAddress({ StreetName: format.StreetName, StreetNumber: format.StreetNumber, ZipCode: format.ZipCode, Town: format.Town }).$promise.then(function (res) {
                    addr.Latitude = res[0].Latitude;
                    addr.Longitude = res[0].Longitude;
                    mapArray.push({ name: addr.Name, lat: addr.Latitude, lng: addr.Longitude });
                });
            }
        }


        var setMap = function(mapArray) {
            OS2RouteMap.set(mapArray);
        }
    }
]);