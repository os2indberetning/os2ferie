angular.module("application").controller("DrivingController", [
    "$scope", "SmartAdresseSource", "DriveReport", "PersonalAddress", "AddressFormatter", "PersonalAddressType", "Person", function ($scope, SmartAdresseSource, DriveReport, PersonalAddress, AddressFormatter, PersonalAddressType, Person) {

        $scope.DriveReport = new DriveReport();

        $scope.Person = Person.get({ id: 1 });

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

        $scope.Save = function() {
            console.log($scope.DriveReport);

            // Prepare all data to  be uploaded

            /*
              DriveReport
                  Distance
                  AmountToReimburse
                  Purpose
                  KmRate  // Transportmiddel skal sættes til Rate
                  DriveDateTimestamp
                  FourKmRule
                  StartsAtHome
                  EndsAtHome
                  Licenseplate
                  Fullname
                  DriveReportPoints
                    
                  KilometerAllowance
                
             */


            /*
                PersonalAddress 
                
                    PersonId = required
                    Type = 0
                        Standard = 0,
                        Home = 1,
                        Work = 2,
                        AlternativeHome = 3,
                        AlternativeWork = 4
                    Id = null
                    StreetName = required
                    StreetNumber = required
                    ZipCode = required 
                    Town = required
                    Longitude = null
                    Latitude = null
                    Description = null
                    
             */

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


            var report = $scope.DriveReport;


            

            //$scope.DriveReport.$save(function(response) {
            //    console.log(response);
            //});
        };

        $scope.AddDestination = function() {
            $scope.DriveReport.Addresses.push({ Name: "", Save: false });
        };

        $scope.Remove = function (array, index) {
            array.splice(index, 1);
        };
    }
]);