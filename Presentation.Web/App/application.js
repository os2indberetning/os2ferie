var application = angular.module("application", ["kendo.directives", "ui.router", "ui.bootstrap", "ui.bootstrap.tooltip", "ngResource", "template/modal/window.html", "template/modal/window.html", "template/modal/backdrop.html", "template/tabs/tab.html", "template/tabs/tabset.html", "angularMoment", "template/popover/popover.html", "kendo-ie-fix"]);

angular.module("application").config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/");

    $stateProvider
        .state("Default", {
            url: "/",
            templateUrl: "/App/Driving/DrivingView.html",
            controller: "DrivingController",
            resolve: {
                ReadReportCommentHelp: ['HelpText', function (HelpText) {
                    return HelpText.get({ id: "ReadReportCommentHelp" }).$promise.then(function (res) {
                        return res.text;
                    });
                }],

                HomeAddress: ['PersonalAddress', function (PersonalAddress) {
                    return PersonalAddress.GetHomeForUser({ id: 1 }).$promise.then(function (res) {
                        return res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode;
                    });
                }
                ],

                Person: ['Person', function (Person) {

                    return Person.get({ id: 1 }).$promise;
                }],

                LatestDriveReport: ['DriveReport', function (DriveReport) {
                    return DriveReport.getLatest({ id: 1 }).$promise;
                }],

                PersonEmployments: ['PersonEmployments', function (PersonEmployments) {

                    return PersonEmployments.get({ id: 1 }).$promise.then(function (res) {
                        angular.forEach(res, function (employment, key) {
                            employment.PresentationString = employment.Position + " - " + employment.OrgUnit.ShortDescription;
                        });
                        return res;
                    });
                }],

                ThisYearsRates: ['Rate', function (Rate) {
                    return Rate.ThisYearsRates().$promise;
                }],

                LicensePlates: ['LicensePlate', function (LicensePlate) {
                    return LicensePlate.get({ id: 1 }).$promise.then(function (res) {
                        if (res.length > 0) {
                            angular.forEach(res, function (plate, key) {
                                plate.PresentationString = plate.Plate + " - " + plate.Description;
                            });
                            return res;
                        } else {
                            return [{ PresentationString: "Ingen nummerplade" }];
                        }
                    });
                }],


                PersonalAndStandardAddresses: ['Address', function (Address) {
                    return Address.GetPersonalAndStandard({ personId: 1 }).$promise.then(function (res) {
                        var temp = [{ value: "Vælg fast adresse" }];
                        angular.forEach(res, function (value, key) {
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
                        return temp;
                    });
                }],

                Routes: ['Route', function (Route) {
                    return Route.get({ query: "&filter=PersonId eq 1" }).$promise.then(function (res) {
                        angular.forEach(res.value, function (value, key) {
                            var presentation = "";
                            if (value.Description.length > 0) {
                                presentation = value.Description + " : ";
                            }
                            presentation += value.Points[0].StreetName + " ";
                            presentation += value.Points[0].StreetNumber + ", ";
                            presentation += value.Points[0].ZipCode + " ";
                            presentation += value.Points[0].Town + " -> ";
                            presentation += value.Points[value.Points.length - 1].StreetName + " ";
                            presentation += value.Points[value.Points.length - 1].StreetNumber + ", ";
                            presentation += value.Points[value.Points.length - 1].ZipCode + " ";
                            presentation += value.Points[value.Points.length - 1].Town + ". ";
                            presentation += "Antal viapunkter: " + (value.Points.length - 1).toString();
                            value.presentation = presentation;
                        });


                        res.value.unshift({ Name: "", Personal: "", presentation: "" });
                        return res.value;
                    });
                }],

                MapStartAddress: ['Address', function (Address) {
                    return Address.getMapStart().$promise.then(function (res) {
                        return [
                            { name: res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town, lat: res.Latitude, lng: res.Longitude },
                            { name: res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town, lat: res.Latitude, lng: res.Longitude }
                        ];
                    });
                }],


                
}
        })
        .state("driving", {
            url: "/driving",
            templateUrl: "/App/Driving/DrivingView.html",
            controller: "DrivingController",
            resolve: {
                ReadReportCommentHelp: ['HelpText', function (HelpText) {
                    return HelpText.get({ id: "ReadReportCommentHelp" }).$promise.then(function (res) {
                        return res.text;
                    });
                }],

                HomeAddress: ['PersonalAddress', function (PersonalAddress) {
                    return PersonalAddress.GetHomeForUser({ id: 1 }).$promise.then(function (res) {
                        return res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode;
                    });
                }
                ],

                Person: ['Person', function (Person) {
                    return Person.get({ id: 1 }).$promise;
                }],

                LatestDriveReport: ['DriveReport', function (DriveReport) {
                    return DriveReport.getLatest({ id: 1 }).$promise;
                }],

                PersonEmployments: ['PersonEmployments', function (PersonEmployments) {

                    return PersonEmployments.get({ id: 1 }).$promise.then(function (res) {
                        angular.forEach(res, function (employment, key) {
                            employment.PresentationString = employment.Position + " - " + employment.OrgUnit.ShortDescription;
                        });
                        return res;
                    });
                }],

                ThisYearsRates: ['Rate', function (Rate) {
                    return Rate.ThisYearsRates().$promise;
                }],

                LicensePlates: ['LicensePlate', function (LicensePlate) {
                    return LicensePlate.get({ id: 1 }).$promise.then(function (res) {
                        if (res.length > 0) {
                            angular.forEach(res, function (plate, key) {
                                plate.PresentationString = plate.Plate + " - " + plate.Description;
                            });
                            return res;
                        } else {
                            return [{ PresentationString: "Ingen nummerplade" }];
                        }
                    });
                }],


                PersonalAndStandardAddresses: ['Address', function (Address) {
                    return Address.GetPersonalAndStandard({ personId: 1 }).$promise.then(function (res) {
                        var temp = [{ value: "Vælg fast adresse" }];
                        angular.forEach(res, function (value, key) {
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
                        return temp;
                    });
                }],

                Routes: ['Route', function (Route) {
                    return Route.get({ query: "&filter=PersonId eq 1" }).$promise.then(function (res) {
                        angular.forEach(res.value, function (value, key) {
                            var presentation = "";
                            if (value.Description.length > 0) {
                                presentation = value.Description + " : ";
                            }
                            presentation += value.Points[0].StreetName + " ";
                            presentation += value.Points[0].StreetNumber + ", ";
                            presentation += value.Points[0].ZipCode + " ";
                            presentation += value.Points[0].Town + " -> ";
                            presentation += value.Points[value.Points.length - 1].StreetName + " ";
                            presentation += value.Points[value.Points.length - 1].StreetNumber + ", ";
                            presentation += value.Points[value.Points.length - 1].ZipCode + " ";
                            presentation += value.Points[value.Points.length - 1].Town + ". ";
                            presentation += "Antal viapunkter: " + (value.Points.length - 1).toString();
                            value.presentation = presentation;
                        });


                        res.value.unshift({ Name: "", Personal: "", presentation: "" });
                        return res.value;
                    });
                }],

                MapStartAddress: ['Address', function (Address) {
                    return Address.getMapStart().$promise.then(function (res) {
                        return [
                            { name: res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town, lat: res.Latitude, lng: res.Longitude },
                            { name: res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town, lat: res.Latitude, lng: res.Longitude }
                        ];
                    });
                }],
                

        }})
        .state("myreports", {
            url: "/myreports",
            templateUrl: "/App/MyReports/MyReportsView.html"
        })
        .state("approvereports", {
            url: "/approvereports",
            templateUrl: "/App/ApproveReports/ApproveReportsView.html",
            resolve: {
                CurrentUser: ["Person", "$location", function (Person, $location) {
                    return Person.GetCurrentUser().$promise.then(function (data) {
                        if (!data.IsLeader) {
                            $location.path("driving");
                        }
                    });
                }]
            }
        })
        .state("settings", {
            url: "/settings",
            templateUrl: "/App/Settings/SettingsView.html"
        })
        .state("admin", {
            url: "/admin",
            templateUrl: "/App/Admin/AdminView.html",
            resolve: {
                CurrentUser: ["Person", "$location", function(Person, $location) {
                    return Person.GetCurrentUser().$promise.then(function(data) {
                        if (!data.IsAdmin) {
                            $location.path("driving");
                        }
                    });
                }]
            }
        });
}]);

application.constant('angularMomentConfig', {
    preprocess: 'utc',
    timezone: 'Europe/Copenhagen'
});