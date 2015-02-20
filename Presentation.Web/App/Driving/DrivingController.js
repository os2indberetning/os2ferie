angular.module("application").controller("DrivingController", [
    "$scope", "SmartAdresseSource", "DriveReport", "PersonalAddress", "AddressFormatter", "PersonalAddressType", "Person", "PersonEmployments", "Rate", function ($scope, SmartAdresseSource, DriveReport, PersonalAddress, AddressFormatter, PersonalAddressType, Person, PersonEmployments, Rate) {

        $scope.DriveReport = new DriveReport();

        $scope.Person = Person.get({ id: 1 });
        $scope.FourKmRule = {}
        $scope.FourKmRule.Using = false;

        $scope.KmRate = Rate.ThisYearsRates(function () {
            $scope.KmRateDropDown.dataSource.read();
        });


        $scope.Employments = PersonEmployments.get({ id: 1 }, function () {
            $scope.PositionDropDown.dataSource.read();
        });


        $scope.DriveReport.Addresses = [];

        $scope.DriveReport.Addresses.push({ Name: "", Save: false });
        $scope.DriveReport.Addresses.push({ Name: "", Save: false });

        $scope.SmartAddress = SmartAdresseSource;

        $scope.DriveReport.Date = new Date();

        $scope.DateOptions = {
            start: "month"
        };


        $scope.DrivenKilometers = 33;
        $scope.TransportAllowance = 33;
        $scope.RemainingKilometers = 0;
        $scope.PayoutAmount = 123;

        $scope.Save = function () {

            var driveReport = new DriveReport();

            // Prepare all data to  be uploaded
            driveReport.Purpose = $scope.DriveReport.Purpose;
            driveReport.DriveDateTimestamp = Math.floor($scope.DriveReport.Date.getTime() / 1000);
            driveReport.KmRate = parseFloat($scope.DriveReport.KmRate);
            driveReport.KilometerAllowance = $scope.DriveReport.KilometerAllowance;

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

                    var currentAddress = new PersonalAddress(AddressFormatter.fn(address.Name));

                    //delete currentAddress.Id;
                    //currentAddress.Description = "";
                    //currentAddress.Longitude = "";
                    //currentAddress.Latitude = "";
                    //currentAddress.DriveReportId = 0;
                    //currentAddress.NextPointId = 0;
                    //currentAddress.PreviousPointId = 0;

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
                        console.log(address);

                        var personalAddress = new PersonalAddress(AddressFormatter.fn(address.Name));

                        console.log($scope.Person);

                        personalAddress.PersonId = $scope.Person.Id;
                        personalAddress.Type = PersonalAddressType.Standard;
                        personalAddress.Longitude = "";
                        personalAddress.Latitude = "";
                        personalAddress.Description = "";



                        delete personalAddress.Id;

                        personalAddress.$save(function (response) {
                            console.log(response);
                        });

                    }

                });

            }



            if (typeof $scope.DriveReport.FourKmRule !== "undefined" && $scope.DriveReport.FourKmRule.Using === true) {
                driveReport.FourKmRule = true;
            } else {
                driveReport.FourKmRule = false;
            }

            driveReport.Licenseplate = "";
            driveReport.Distance = 0;
            driveReport.AmountToReimburse = 0;
            driveReport.Fullname = "";
            driveReport.Timestamp = "";




            driveReport.PersonId = $scope.Person.Id;
            driveReport.status = "Reported";
            driveReport.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
            driveReport.EditedDateTimestamp = driveReport.CreatedDateTimestamp;

            driveReport.Comment = "";
            driveReport.ClosedDateTimestamp = 0;
            driveReport.ProcessedDateTimestamp = 0;
            driveReport.EmploymentId = $scope.DriveReport.Position;


            // Not working when points are set.
            //delete driveReport.DriveReportPoints;
            


            console.log(driveReport);

            // Jobs done! Send to backend
            driveReport.$save(function (response) {
                console.log(response);
            });
        };

        $scope.AddViapoint = function () {
            var temp = $scope.DriveReport.Addresses.pop();
            $scope.DriveReport.Addresses.push({ Name: "", Save: false });
            $scope.DriveReport.Addresses.push(temp);
        };

        $scope.Remove = function (array, index) {
            array.splice(index, 1);
        };
    }
]);