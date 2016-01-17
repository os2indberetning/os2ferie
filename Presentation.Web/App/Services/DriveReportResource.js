angular.module("application").service('DriveReport', ["$resource","AddressFormatter", "PersonalAddress", "$modal", "PersonalAddressType", function ($resource, AddressFormatter, PersonalAddress, $modal, PersonalAddressType) {
    return $resource("/odata/DriveReports(:id)?:query&emailText=:emailText", { id: "@id", query: "@query", emailText: "@emailText" }, {
        "get": {
            method: "GET", isArray: false, transformResponse: function (res) {
            return angular.fromJson(res).value[0];
        } },
        "create": {
            method: "POST",
            isArray: false,
            transformRequest: function ($scope) {

                var getKmRate = function () {
                    for (var i = 0; i < $scope.KmRate.length; i++) {
                        if ($scope.KmRate[i].Id == $scope.DriveReport.KmRate) {
                            return $scope.KmRate[i];
                        }
                    }
                };

                var driveReport = {};

                // Prepare all data to  be uploaded
                driveReport.Purpose = $scope.DriveReport.Purpose;
                driveReport.DriveDateTimestamp = Math.floor($scope.DriveReport.Date.getTime() / 1000);
                driveReport.KmRate = parseFloat(getKmRate().KmRate);
                driveReport.TFCode = getKmRate().Type.TFCode;
                driveReport.IsRoundTrip = $scope.DriveReport.IsRoundTrip;

                driveReport.KilometerAllowance = $scope.DriveReport.KilometerAllowance;
                driveReport.Distance = 0;
                driveReport.AmountToReimburse = 0;

                if ($scope.showLicensePlate) {
                    driveReport.LicensePlate = $scope.DriveReport.LicensePlate;
                } else {
                    driveReport.LicensePlate = "0000000";
                }


                driveReport.PersonId = $scope.currentUser.Id;
                driveReport.FullName = $scope.currentUser.FullName;
                driveReport.Status = "Pending";
                driveReport.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
                driveReport.EditedDateTimestamp = driveReport.CreatedDateTimestamp;
                driveReport.Comment = "";
                driveReport.ClosedDateTimestamp = 0;
                driveReport.ProcessedDateTimestamp = 0;
                driveReport.EmploymentId = $scope.DriveReport.Position;

                if ($scope.DriveReport.KilometerAllowance === "Read") {

                    driveReport.Distance = Number($scope.DriveReport.ReadDistance.toString().replace(",", "."));
                    if (Number(driveReport.Distance) < 0) {
                        driveReport.Distance = 0;
                    }
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

                        if (address.Latitude == undefined) {
                            address.Latitude = "";
                        }
                        if (address.Longitude == undefined) {
                            address.Longitude = "";
                        }

                        driveReport.DriveReportPoints.push({
                            StreetName: currentAddress.StreetName,
                            StreetNumber: currentAddress.StreetNumber,
                            ZipCode: currentAddress.ZipCode,
                            Town: currentAddress.Town,
                            Description: "",
                            Latitude: address.Latitude.toString(),
                            Longitude: address.Longitude.toString()
                        });
                    });

                    // go through addresses and see which is going to be saved
                    angular.forEach($scope.DriveReport.Addresses, function (address, key) {

                        if (address.Save) {
                            var personalAddress = new PersonalAddress(AddressFormatter.fn(address.Name));

                            personalAddress.PersonId = $scope.currentUser.Id;
                            personalAddress.Type = PersonalAddressType.Standard;
                            personalAddress.Longitude = "";
                            personalAddress.Latitude = "";
                            personalAddress.Description = "";

                            delete personalAddress.Id;

                            personalAddress.$save(function (res) {
                                res.PresentationString = res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town,
                                res.address = res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town

                                $scope.PersonalAddresses.push(res);
                                angular.forEach($scope.container.PersonalAddressDropDown, function (entry, innerKey) {
                                    entry.dataSource.read();
                                });
                                
                            });

                            
                        }
                    });

                    
                    
                }

                if (typeof $scope.DriveReport.FourKmRule !== "undefined" && $scope.DriveReport.FourKmRule.Using === true) {
                    driveReport.FourKmRule = true;
                } else {
                    driveReport.FourKmRule = false;
                }
                return JSON.stringify(driveReport);
            }
        }, "edit": {

            method: "POST",
            isArray: false,
            url: "/odata/DriveReports(:id)?:query&emailText=:emailText",
            transformRequest: function ($scope) {

                var getKmRate = function () {
                    for (var i = 0; i < $scope.KmRate.length; i++) {
                        if ($scope.KmRate[i].Id == $scope.DriveReport.KmRate) {
                            return $scope.KmRate[i];
                        }
                    }
                };

                var driveReport = {};

                // Prepare all data to  be uploaded
                driveReport.Purpose = $scope.DriveReport.Purpose;
                driveReport.DriveDateTimestamp = Math.floor($scope.DriveReport.Date.getTime() / 1000);
                driveReport.KmRate = parseFloat(getKmRate().KmRate);
                driveReport.TFCode = getKmRate().Type.TFCode;
                driveReport.IsRoundTrip = $scope.DriveReport.IsRoundTrip;

                driveReport.KilometerAllowance = $scope.DriveReport.KilometerAllowance;
                driveReport.Distance = 0;
                driveReport.AmountToReimburse = 0;

                if ($scope.showLicensePlate) {
                    driveReport.LicensePlate = $scope.DriveReport.LicensePlate;
                } else {
                    driveReport.LicensePlate = "0000000";
                }

                driveReport.ApprovedById = $scope.latestDriveReport.ApprovedById;

                driveReport.PersonId = $scope.latestDriveReport.PersonId;
                driveReport.FullName = $scope.latestDriveReport.FullName;
                driveReport.Status = $scope.latestDriveReport.Status;
                driveReport.CreatedDateTimestamp = $scope.latestDriveReport.CreatedDateTimestamp;
                driveReport.EditedDateTimestamp = Math.floor(Date.now() / 1000);
                driveReport.Comment = "";
                driveReport.ClosedDateTimestamp = $scope.latestDriveReport.ClosedDateTimestamp;
                driveReport.ProcessedDateTimestamp = $scope.latestDriveReport.ProcessedDateTimestamp;
                driveReport.EmploymentId = $scope.DriveReport.Position;

                if ($scope.DriveReport.KilometerAllowance === "Read") {

                    driveReport.Distance = Number($scope.DriveReport.ReadDistance.toString().replace(",", "."));
                    if (Number(driveReport.Distance) < 0) {
                        driveReport.Distance = 0;
                    }
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

                        if (address.Latitude == undefined) {
                            address.Latitude = "";
                        }
                        if (address.Longitude == undefined) {
                            address.Longitude = "";
                        }

                        driveReport.DriveReportPoints.push({
                            StreetName: currentAddress.StreetName,
                            StreetNumber: currentAddress.StreetNumber,
                            ZipCode: currentAddress.ZipCode,
                            Town: currentAddress.Town,
                            Description: "",
                            Latitude: address.Latitude.toString(),
                            Longitude: address.Longitude.toString()
                        });
                    });

                    // go through addresses and see which is going to be saved
                    angular.forEach($scope.DriveReport.Addresses, function (address, key) {

                        if (address.Save) {
                            var personalAddress = new PersonalAddress(AddressFormatter.fn(address.Name));

                            personalAddress.PersonId = $scope.currentUser.Id;
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
                return JSON.stringify(driveReport);
            }
        },
        "patch": { method: "PATCH" },
        "getLatest": {
            method: "GET",
            isArray: false,
            url: "/odata/DriveReports/Service.GetLatestReportForUser?personId=:id",
            transformResponse: function (data) {
                var res = angular.fromJson(data);
                return res;
            }
        },
        "getWithPoints": {
            method: "GET",
            isArray: false,
            url: "/odata/DriveReports?$filter=Id eq :id &$expand=DriveReportPoints",
            transformResponse: function(data) {
                var res = angular.fromJson(data);
                if (res.error == undefined) {
                    return res.value[0];
                }

                var modalInstance = $modal.open({
                    templateUrl: '/App/Services/Error/ServiceError.html',
                    controller: "ServiceErrorController",
                    backdrop: "static",
                    resolve: {
                        errorMsg: function () {
                            return res.error.innererror.message;
                        }
                    }
                });
                return res;
            }
        }
    });
}]);