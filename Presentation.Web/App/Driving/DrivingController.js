angular.module("application").controller("DrivingController", [
    "$scope", "SmartAdresseSource", "DriveReport", "PersonalAddress", "AddressFormatter", "PersonalAddressType", "Person", "PersonEmployments", "Rate", function ($scope, SmartAdresseSource, DriveReport, PersonalAddress, AddressFormatter, PersonalAddressType, Person, PersonEmployments, Rate) {

        $scope.DriveReport = new DriveReport();

        $scope.Person = Person.get({ id: 1 });
        $scope.FourKmRule = {}
        $scope.FourKmRule.Using = false;

        $scope.KmRate = Rate.ThisYearsRates(function() {
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
            driveReport.DriveDateTimestamp = $scope.DriveReport.Date.getTime();
            driveReport.KmRate = $scope.DriveReport.KmRate;
            driveReport.KilometerAllowance = $scope.DriveReport.KilometerAllowance;

            if ($scope.DriveReport.KilometerAllowance === "Read") {
                if ($scope.DriveReport.StartOrEndedAtHome === 'Started') {
                    driveReport.StartsAtHome = true;
                    driveReport.StartsAtWork = false;
                }
                else if ($scope.DriveReport.StartOrEndedAtHome === 'Ended') {
                    driveReport.StartsAtHome = false;
                    driveReport.StartsAtWork = true;
                } 
                else if ($scope.DriveReport.StartOrEndedAtHome === 'Both') {
                    driveReport.StartsAtHome = true;
                    driveReport.StartsAtWork = true;
                }
                else {
                    driveReport.StartsAtHome = false;
                    driveReport.StartsAtWork = false;
                }
            }

            if (typeof $scope.DriveReport.FourKmRule !== "undefined") {
                if ($scope.DriveReport.FourKmRule.Using === true) {
                    driveReport.Distance = $scope.DriveReport.FourKmRule.Value;
                } 
            } 


            /*
              DriveReport
                  Distance **
                  AmountToReimburse **
                  Purpose **
                  KmRate  ** // Transportmiddel skal sættes til Rate
                  DriveDateTimestamp **
                  FourKmRule **
                  StartsAtHome **
                  EndsAtHome **
                  Licenseplate
                  Fullname **
                  DriveReportPoints 
                  KilometerAllowance **


             */

            var previousAddress;

            $scope.DriveReport.DriveReportPoints = [];

            angular.forEach($scope.DriveReport.Addresses, function (address, key) {

                var currentAddress = new PersonalAddress(AddressFormatter.fn(address.Name));

                if (previousAddress != null) {
                    previousAddress.NextPoint = currentAddress;
                    //currentAddress.PreviousPoint = previousAddress;
                }

                $scope.DriveReport.DriveReportPoints.push(currentAddress);
                previousAddress = currentAddress;

            });

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

                    personalAddress.$save(function(response) {
                        console.log(response);
                    });

                }

            });


            console.log(driveReport);


            driveReport.$save(function(response) {
                console.log(response);
            });
        };

        $scope.AddDestination = function() {
            $scope.DriveReport.Addresses.push({ Name: "", Save: false });
        };

        $scope.Remove = function (array, index) {
            array.splice(index, 1);
        };
    }
]);