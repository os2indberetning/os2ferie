angular.module("application").controller("SettingController", [
    "$scope", "$modal", "Person", "LicensePlate", "Personalroute", "Point", "Address", "Route", "AddressFormatter", "$http", "NotificationService", "Token", "SmartAdresseSource", function ($scope, $modal, Person, LicensePlate, Personalroute, Point, Address, Route, AddressFormatter, $http, NotificationService, Token, SmartAdresseSource) {
        $scope.gridContainer = {};
        $scope.isCollapsed = true;
        $scope.mailAdvice = '';
        $scope.licenseplates = [];
        $scope.newLicensePlate = "";
        $scope.newLicensePlateDescription = "";
        $scope.workDistanceOverride = 0;
        $scope.recieveMail = false;
        $scope.oldAlternativeHomeAddress = "";
        $scope.oldAlternativeHomeAddressId = 0;
        $scope.newAlternativeHomeAddress = "";
        $scope.oldAlternativeWorkAddress = "";
        $scope.oldAlternativeWorkAddressId = 0;
        $scope.newAlternativeWorkAddress = "";
        $scope.routes = [];
        $scope.addresses = [];
        $scope.tokens = [];
        $scope.isCollapsed = true;
        $scope.tokenIsCollapsed = true;
        $scope.newTokenDescription = "";

        // Hardcoded personId
        var personId = 1;

        // Contains references to kendo ui grids.
        $scope.gridContainer = {};

        $scope.GetPerson = Person.get({ id: personId }, function (data) {
            $scope.currentPerson = data;
            $scope.workDistanceOverride = $scope.currentPerson.WorkDistanceOverride.toString().replace('.', ',');
            $scope.recieveMail = data.RecieveMail;

            //Set choice of mail notification
            if ($scope.recieveMail == true) {
                $scope.mailAdvice = 'Yes';
            } else {
                $scope.mailAdvice = 'No';
            }

            //Load licenseplates
            LicensePlate.get({ id: personId }, function (data) {
                $scope.licenseplates = data;
            });

            $scope.loadAlternativeHomeAddress();
            $scope.loadAlternativeWorkAddress();

            //NotificationService.AutoFadeNotification("success", "Success", "Person fundet");
        }, function () {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Person ikke fundet");
        });

        //Load alternative home address
        $scope.loadAlternativeHomeAddress = function () {
            Address.get({ query: "$filter=Type eq Core.DomainModel.PersonalAddressType'AlternativeHome' and PersonId eq " + $scope.currentPerson.Id }, function (data) {
                if (data.value[0] != undefined) {
                    $scope.oldAlternativeHomeAddressId = data.value[0].Id;
                    $scope.oldAlternativeHomeAddress = data.value[0].StreetName + " " + data.value[0].StreetNumber + ", " + data.value[0].ZipCode + " " + data.value[0].Town;
                    $scope.newAlternativeHomeAddress = $scope.oldAlternativeHomeAddress;
                } else {
                    $scope.newAlternativeHomeAddress = "";
                    $scope.oldAlternativeHomeAddress = "";
                }
            }, function () {
                // Error loading alternative home address.
            });
        }

        //Load alternative work address
        $scope.loadAlternativeWorkAddress = function () {
            Address.get({ query: "$filter=Type eq Core.DomainModel.PersonalAddressType'AlternativeWork' and PersonId eq " + $scope.currentPerson.Id }, function (data) {
                if (data.value[0] != undefined) {
                    $scope.oldAlternativeWorkAddressId = data.value[0].Id;
                    $scope.oldAlternativeWorkAddress = data.value[0].StreetName + " " + data.value[0].StreetNumber + ", " + data.value[0].ZipCode + " " + data.value[0].Town;
                    $scope.newAlternativeWorkAddress = $scope.oldAlternativeWorkAddress;
                } else {
                    $scope.newAlternativeWorkAddress = "";
                    $scope.oldAlternativeWorkAddress = "";
                }
            }, function () {
                // Error loading alternative work address.
            });
        }

        //Funtionalitet til opslag af adresser
        $scope.SmartAddress = SmartAdresseSource;


        //Gem ny nummerplade
        $scope.saveNewLicensePlate = function () {
            var plate = "";

            var array = $scope.newLicensePlate.split('');

            if ($scope.newLicensePlateDescription == "") {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Nummerplade skal have en beskrivelse");
                return;
            }

            if (array.length < 7 || array.length > 9) {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Nummerplade er ikke i korrekt format. Eksempel: AB 12 345");
                return;
            }

            var first = array[0];
            var second = array[1];

            array[0] = first.toUpperCase();
            array[1] = second.toUpperCase();

            var cleanArray = [];
            if (array.length >= 7 || array.length <= 9) {
                for (var i = 0; i < array.length; i++) {
                    if (array[i] != ' ') {
                        cleanArray[i] = array[i];
                    }
                }
            }

            cleanArray.splice(2, 0, ' ');
            cleanArray.splice(5, 0, ' ');

            plate = cleanArray.join("");

            var newPlate = new LicensePlate({
                Plate: plate,
                Description: $scope.newLicensePlateDescription,
                PersonId: personId
            });

            newPlate.$save(function (data) {
                $scope.licenseplates.push(data);
                $scope.licenseplates.sort(function (a, b) {
                    return a.Id > b.Id;
                });
                $scope.newLicensePlate = "";
                $scope.newLicensePlateDescription = "";

                NotificationService.AutoFadeNotification("success", "Success", "Ny nummerplade blev gemt");
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Nummerplade blev ikke gemt");
            });
        };

        //Slet eksisterende nummerplade
        $scope.deleteLicensePlate = function (plate) {
            LicensePlate.delete({ id: plate.Id }, function () {
                NotificationService.AutoFadeNotification("success", "Success", "Nummerplade blev slettet");
                //Load licenseplates again
                LicensePlate.get({ id: personId }, function (data) {
                    $scope.licenseplates = data;
                });
            }), function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Nummerplade blev ikke slettet");
            };
        }

        //Skift valg om mailnotifikationer
        $scope.invertRecieveMail = function () {
            $scope.recieveMail = !$scope.recieveMail;

            var newPerson = new Person({
                RecieveMail: $scope.recieveMail
            });

            newPerson.$patch({ id: personId }, function () {
                NotificationService.AutoFadeNotification("success", "Success", "Valg om modtagelse af mails blev gemt");
            }), function () {
                $scope.recieveMail = !$scope.recieveMail;
                NotificationService.AutoFadeNotification("danger", "Fejl", "Valg om modtagelse af mails blev ikke gemt");
            };
        }

        //Funktionalitet til alternativ hjemmeadresse
        $scope.saveAlternativeHomeAddress = function () {
            if ($scope.oldAlternativeHomeAddress == "") { // CREATE IT
                var result = AddressFormatter.fn($scope.newAlternativeHomeAddress);

                result.Id = $scope.oldAlternativeHomeAddressId;
                result.PersonId = $scope.currentPerson.Id;

                var newAlternativeHomeAddress = new Address({
                    Id: result.Id,
                    PersonId: result.PersonId,
                    StreetName: result.StreetName,
                    StreetNumber: result.StreetNumber,
                    ZipCode: result.ZipCode,
                    Town: result.Town,
                    Type: "AlternativeHome",
                    Latitude: "",
                    Longitude: ""
                });

                newAlternativeHomeAddress.$post({}, function (data) {
                    $scope.loadAlternativeHomeAddress();
                    NotificationService.AutoFadeNotification("success", "Success", "Alternativ hjemmeadresse gemt");

                    // Save HomeWorkOverride as 0 when saving home address.
                    $scope.workDistanceOverride = 0;
                    $scope.setHomeWorkOverride();
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "Fejl", "Alternativ hjemmeadresse kunne ikke gemmes");
                });

            } else if ($scope.newAlternativeHomeAddress == "") { // DELETE IT
                Address.delete({ id: $scope.oldAlternativeHomeAddressId }, function () {
                    $scope.loadAlternativeHomeAddress();
                    NotificationService.AutoFadeNotification("success", "Success", "Alternativ hjemmeadresse slettet");
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "Fejl", "Alternativ hjemmeadresse kunne ikke slettes");
                });

            } else if ($scope.newAlternativeHomeAddress != $scope.oldAlternativeHomeAddress) { // UPDATE IT                
                var result = AddressFormatter.fn($scope.newAlternativeHomeAddress);

                result.Id = $scope.oldAlternativeHomeAddressId;
                result.PersonId = $scope.currentPerson.Id;

                var editedAlternativeHomeAddress = new Address({
                    Id: result.Id,
                    PersonId: result.PersonId,
                    StreetName: result.StreetName,
                    StreetNumber: result.StreetNumber,
                    ZipCode: result.ZipCode,
                    Town: result.Town,
                    Description: result.Description,
                    Type: "AlternativeHome"
                });

                editedAlternativeHomeAddress.$patch({ id: result.Id }, function (data) {
                    $scope.loadAlternativeHomeAddress();
                    NotificationService.AutoFadeNotification("success", "Success", "Alternativ hjemmeadresse opdateret");

                    // Save HomeWorkOverride as 0 when saving home address.
                    $scope.workDistanceOverride = 0;
                    $scope.setHomeWorkOverride();
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "Fejl", "Alternativ hjemmeadresse blev ikke opdateret");
                });
            }
        }

        //Funktionalitet til alternativ arbejdsadresse
        $scope.saveAlternativeWorkAddress = function () {
            if ($scope.oldAlternativeWorkAddress == "") { // CREATE IT
                var result = AddressFormatter.fn($scope.newAlternativeWorkAddress);

                result.Id = $scope.oldAlternativeWorkAddressId;
                result.PersonId = $scope.currentPerson.Id;

                var newAlternativeWorkAddress = new Address({
                    Id: result.Id,
                    PersonId: result.PersonId,
                    StreetName: result.StreetName,
                    StreetNumber: result.StreetNumber,
                    ZipCode: result.ZipCode,
                    Town: result.Town,
                    Type: "AlternativeWork",
                    Latitude: "",
                    Longitude: ""
                });

                newAlternativeWorkAddress.$post({}, function (data) {
                    $scope.loadAlternativeWorkAddress();
                    NotificationService.AutoFadeNotification("success", "Success", "Alternativ arbejdsadresse gemt");

                    // Save HomeWorkOverride as 0 when saving work address.
                    $scope.workDistanceOverride = 0;
                    $scope.setHomeWorkOverride();

                }, function () {
                    NotificationService.AutoFadeNotification("danger", "Fejl", "Alternativ arbejdsadresse kunne ikke gemmes");
                });

            } else if ($scope.newAlternativeWorkAddress == "") { // DELETE IT
                Address.delete({ id: $scope.oldAlternativeWorkAddressId }, function () {
                    $scope.loadAlternativeWorkAddress();
                    NotificationService.AutoFadeNotification("success", "Success", "Alternativ arbejdsadresse slettet");
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "Fejl", "Alternativ arbejdsadresse kunne ikke slettes");
                });

            } else if ($scope.newAlternativeWorkAddress != $scope.oldAlternativeWorkAddress) { // UPDATE IT
                var result = AddressFormatter.fn($scope.newAlternativeWorkAddress);

                result.Id = $scope.oldAlternativeWorkAddressId;
                result.PersonId = $scope.currentPerson.Id;

                var editedAlternativeWorkAddress = new Address({
                    Id: result.Id,
                    PersonId: result.PersonId,
                    StreetName: result.StreetName,
                    StreetNumber: result.StreetNumber,
                    ZipCode: result.ZipCode,
                    Town: result.Town,
                    Description: result.Description,
                    Type: "AlternativeWork"
                });

                editedAlternativeWorkAddress.$patch({ id: result.Id }, function (data) {
                    $scope.loadAlternativeWorkAddress();
                    NotificationService.AutoFadeNotification("success", "Success", "Alternativ hjemmeadresse opdateret");

                    // Save HomeWorkOverride as 0 when saving work address.
                    $scope.workDistanceOverride = 0;
                    $scope.setHomeWorkOverride();
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "Fejl", "Alternativ hjemmeadresse blev ikke opdateret");
                });
            }
        }

        $scope.setHomeWorkOverride = function () {
            var newPerson = new Person({
                WorkDistanceOverride: $scope.workDistanceOverride.toString().replace(',', '.')
            });

            newPerson.$patch({ id: personId }, function (data) {
                NotificationService.AutoFadeNotification("success", "Success", "Afstand mellem hjemme- og arbejdsadresse blev gemt");
            }), function () {
                if ($scope.mailAdvice == 'No') {
                    $scope.mailAdvice = 'Yes';
                } else {
                    $scope.mailAdvice = 'No';
                }
                NotificationService.AutoFadeNotification("danger", "Fejl", "Afstand mellem hjemme- og arbejdsadresse blev ikke gemt");
            };
        };

        $scope.loadGrids = function (id) {
            $scope.personalRoutes = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend: function (req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/PersonalRoutes()?$filter=PersonId eq " + id + "&$expand=Points",
                            dataType: "json",
                            cache: false
                        },
                        parameterMap: function (options, type) {
                            var d = kendo.data.transports.odata.parameterMap(options);

                            delete d.$inlinecount; // <-- remove inlinecount parameter                                                        

                            d.$count = true;

                            return d;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value; // <-- The result is just the data, it doesn't need to be unpacked.
                        },
                        total: function (data) {
                            return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                        }
                    },
                    pageSize: 20,
                    serverPaging: true,
                    serverSorting: true
                },
                sortable: true,
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} personlige ruter", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                        empty: "Ingen personlige ruter at vise",
                        page: "Side",
                        of: "af {0}", //{0} is total amount of pages
                        itemsPerPage: "personlige ruter pr. side",
                        first: "Gå til første side",
                        previous: "Gå til forrige side",
                        next: "Gå til næste side",
                        last: "Gå til sidste side",
                        refresh: "Genopfrisk",
                    },
                    pageSizes: [5, 10, 20, 30, 40, 50]
                },
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "Description",
                        title: "Beskrivelse"
                    }, {
                        field: "Points",
                        template: function (data) {
                            var temp = [];

                            angular.forEach(data.Points, function (value, key) {
                                if (value.PreviousPointId == undefined) {
                                    this.push(value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town);
                                }

                            }, temp);

                            return temp;
                        },
                        title: "Fra"
                    }, {
                        field: "Points",
                        template: function (data) {
                            var temp = [];

                            angular.forEach(data.Points, function (value, key) {
                                if (value.NextPointId == null) {
                                    this.push(value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town);
                                }

                            }, temp);

                            return temp;
                        },
                        title: "Til"
                    },
                    {
                        field: "Id",
                        title: "Muligheder",
                        template: "<a ng-click='openRouteEditModal(${Id})'>Rediger</a> | <a ng-click='openRouteDeleteModal(${Id})'>Slet</a>"
                    }
                ]
            };

            $scope.personalAddresses = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend: function (req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/PersonalAddresses()?$filter=PersonId eq " + personId + " and (Type eq Core.DomainModel.PersonalAddressType'Standard' or Type eq Core.DomainModel.PersonalAddressType'Home' or Type eq Core.DomainModel.PersonalAddressType'Work')",
                            dataType: "json",
                            cache: false
                        },
                        parameterMap: function (options, type) {
                            var d = kendo.data.transports.odata.parameterMap(options);

                            delete d.$inlinecount; // <-- remove inlinecount parameter                                                        

                            d.$count = true;

                            return d;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                        }
                    },
                    pageSize: 5,
                    serverPaging: false,
                    serverSorting: true
                },
                sortable: true,
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} personlige adresser", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                        empty: "Ingen personlige adresser at vise",
                        page: "Side",
                        of: "af {0}", //{0} is total amount of pages
                        itemsPerPage: "personlige adresser pr. side",
                        first: "Gå til første side",
                        previous: "Gå til forrige side",
                        next: "Gå til næste side",
                        last: "Gå til sidste side",
                        refresh: "Genopfrisk",
                    },
                    pageSizes: [5, 10, 20, 30, 40, 50]
                },
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "Description",
                        title: "Beskrivelse",
                        template: function (data) {
                            if (data.Type == "Work") {
                                return "Arbejdsadresse";
                            }
                            else if (data.Type == "Home") {
                                return "Hjemmeadresse";
                            } else return data.Description;
                        }
                    }, {
                        field: "Id",
                        template: function (data) {
                            return (data.StreetName + " " + data.StreetNumber + ", " + data.ZipCode + " " + data.Town);
                        },
                        title: "Adresse"
                    }, {
                        field: "Id",
                        title: "Muligheder",
                        template: function (data) {
                            if (!(data.Type == "Home" || data.Type == "Work")) {
                                return "<a ng-click='openAddressEditModal(" + data.Id + ")'>Rediger</a> | <a ng-click='openAddressDeleteModal(" + data.Id + ")'>Slet</a>";
                            }
                            return "";
                        }
                    }
                ]
            };
        }

        $scope.loadGrids(1);

        $scope.updatePersonalAddresses = function () {
            $scope.gridContainer.personalAddressesGrid.dataSource.transport.options.read.url = "odata/PersonalAddresses()?$filter=PersonId eq " + $scope.currentPerson.Id;
            $scope.gridContainer.personalAddressesGrid.dataSource.read();

        }

        $scope.updatePersonalRoutes = function () {
            $scope.gridcontainer.personalRoutesGrid.dataSource.transport.options.read.url = "odata/PersonalRoutes()?$filter=PersonId eq " + $scope.currentPerson.Id + "&$expand=Points";
            $scope.gridcontainer.personalRoutesGrid.dataSource.read();

        }

        //$scope.openTokenModal = function (size) {

        //    var modalInstance = $modal.open({
        //        templateUrl: '/App/Settings/tokenModal.html',
        //        controller: 'TokenInstanceController',
        //        backdrop: 'static',
        //        size: size,
        //        resolve: {
        //            personId: function () {
        //                return $scope.currentPerson.Id;
        //            }
        //        }
        //    });

        //    modalInstance.result.then(function () {

        //    });
        //};

        $scope.openRouteEditModal = function (id) {

            var modalInstance = $modal.open({
                templateUrl: '/App/Settings/RouteEditModal.html',
                controller: 'RouteEditModalInstanceController',
                backdrop: 'static',
                resolve: {
                    routeId: function () {
                        return id;
                    },
                    personId: function () {
                        return $scope.currentPerson.Id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.updatePersonalRoutes();
            });
        };

        $scope.openRouteAddModal = function (id) {

            var modalInstance = $modal.open({
                templateUrl: '/App/Settings/RouteAddModal.html',
                controller: 'RouteEditModalInstanceController',
                backdrop: 'static',
                resolve: {
                    routeId: function () {
                        return;
                    },
                    personId: function () {
                        return $scope.currentPerson.Id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.updatePersonalRoutes();
            });
        };

        $scope.openRouteDeleteModal = function (id) {

            var modalInstance = $modal.open({
                templateUrl: '/App/Settings/RouteDeleteModal.html',
                controller: 'RouteDeleteModalInstanceController',
                backdrop: 'static',
                resolve: {
                    routeId: function () {
                        return id;
                    },
                    personId: function () {
                        return $scope.currentPerson.Id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.updatePersonalRoutes();
            });
        };

        $scope.openAddressAddModal = function () {

            var modalInstance = $modal.open({
                templateUrl: '/App/Settings/AddressAddModal.html',
                controller: 'AddressEditModalInstanceController',
                backdrop: 'static',
                resolve: {
                    addressId: function () {
                        return;
                    },
                    personId: function () {
                        return $scope.currentPerson.Id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.updatePersonalAddresses();
            });
        };

        $scope.openAddressEditModal = function (id) {

            var modalInstance = $modal.open({
                templateUrl: '/App/Settings/AddressEditModal.html',
                controller: 'AddressEditModalInstanceController',
                backdrop: 'static',
                resolve: {
                    addressId: function () {
                        return id;
                    },
                    personId: function () {
                        return $scope.currentPerson.Id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.updatePersonalAddresses();
            });
        };

        $scope.openAddressDeleteModal = function (id) {

            var modalInstance = $modal.open({
                templateUrl: '/App/Settings/AddressDeleteModal.html',
                controller: 'AddressDeleteModalInstanceController',
                backdrop: 'static',
                resolve: {
                    addressId: function () {
                        return id;
                    },
                    personId: function () {
                        return $scope.currentPerson.Id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.updatePersonalAddresses();
            });
        };

        Token.get({ id: personId }, function (data) {
            $scope.tokens = data.value;
        }, function () {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke hente tokens");
        });

        $scope.deleteToken = function (token) {
            var objIndex = $scope.tokens.indexOf(token);
            $scope.tokens.splice(objIndex, 1);

            Token.delete({ id: token.Id }, function (data) {
                NotificationService.AutoFadeNotification("success", "Success", "Token blev slettet");
            }, function () {
                $scope.tokens.push(token);
                NotificationService.AutoFadeNotification("danger", "Fejl", "Token blev ikke slettet");
            });
        }

        $scope.saveToken = function () {
            var newToken = new Token({
                PersonId: personId,
                Status: "Created",
                Description: $scope.newTokenDescription
            });

            newToken.$save(function (data) {
                $scope.tokens.push(data);
                NotificationService.AutoFadeNotification("success", "Success", "Ny token oprettet");
                $scope.newTokenDescription = "";
                $scope.tokenIsCollapsed = !$scope.tokenIsCollapsed;
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke oprette ny token");
            });
        }

        $scope.newToken = function () {
            $scope.tokenIsCollapsed = !$scope.tokenIsCollapsed;
        }

        $scope.closeTokenModal = function () {
            $modalInstance.close({

            });
        }

        $scope.openConfirmDeleteTokenModal = function (token) {

            var modalInstance = $modal.open({
                templateUrl: '/App/Settings/confirmDeleteTokenModal.html',
                controller: 'confirmDeleteToken',
                backdrop: 'static',
                resolve: {
                    token: function () {
                        return token;
                    }
                }
            });

            modalInstance.result.then(function (tokenToDelete) {
                $scope.deleteToken(tokenToDelete);
            }, function () {

            });
        };

        $scope.makeLicensePlatePrimary = function (plate) {
            LicensePlate.patch({ id: plate.Id }, { IsPrimary: true }, function () {
                //Load licenseplates when finished request.
                LicensePlate.get({ id: personId }, function (data) {
                    $scope.licenseplates = data;
                });
            });
        }
    }
]);