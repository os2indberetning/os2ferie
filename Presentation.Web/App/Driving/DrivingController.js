angular.module("application").controller("DrivingController", [
    "$scope", "SmartAdresseSource", "DriveReport", "PersonalAddress", "AddressFormatter", "PersonalAddressType", "Person", "PersonEmployments", "Rate", "LicensePlate", "NotificationService", "$modal", "$state", "Address", "Route", function ($scope, SmartAdresseSource, DriveReport, PersonalAddress, AddressFormatter, PersonalAddressType, Person, PersonEmployments, Rate, LicensePlate, NotificationService, $modal, $state, Address, Route) {

        $scope.DriveReport = new DriveReport();
        $scope.canSubmitDriveReport = true;
        $scope.Routes = [];
        $scope.IsRoute = false;

        $scope.test = function(e) {
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

            angular.forEach($scope.Routes[index - 1].Points, function(value, key) {
                $scope.DriveReport.Addresses.push({ Name: value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town, Save: false });
            });
        }

        $scope.Person = Person.get({ id: 1 }, function() {
            Address.get({ query: "$filter=PersonId eq " + $scope.Person.Id + " and Type eq Core.DomainModel.PersonalAddressType'Standard'" }, function (data) {
                var temp = [{value: ""}];

                angular.forEach(data.value, function (value, key) {
                    temp.push({ value: value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town });
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

        $scope.Save = function () {

            var driveReport = new DriveReport();

            // Prepare all data to  be uploaded
            driveReport.Purpose = $scope.DriveReport.Purpose;
            driveReport.DriveDateTimestamp = Math.floor($scope.DriveReport.Date.getTime() / 1000);
            driveReport.KmRate = parseFloat($scope.DriveReport.KmRate);
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

                driveReport.Distance = $scope.DriveReport.ReadDistance;

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


                    var tempAddress = (address.Personal.length != 0) ? address.Personal : address.Name;

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
                $scope.DrivenKilometers = response.Distance;
                $scope.TransportAllowance = 0;
                $scope.RemainingKilometers = 0;
                $scope.PayoutAmount = response.AmountToReimburse;

                NotificationService.AutoFadeNotification("success", "Success", "Din kørselsindberetning blev gemt");


            }, function (response) {
                // failure
                NotificationService.AutoFadeNotification("danger", "Fejl", "Din kørselsindberetning blev ikke gemt");
            });
        };

        $scope.AddViapoint = function () {
            var temp = $scope.DriveReport.Addresses.pop();
            $scope.DriveReport.Addresses.push({ Name: "", Personal:"", Save: false });
            $scope.DriveReport.Addresses.push(temp);
        };

        $scope.Remove = function (array, index) {
            array.splice(index, 1);
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
    }
]);