angular.module("app.drive").controller("SettingsController", [
    "$scope", "$modal", "Person", "LicensePlate", "PersonalRoute", "Point", "Address", "Route", "AddressFormatter", "$http", "NotificationService", "Token", "SmartAdresseSource", "$rootScope", "$timeout", "AppLogin",
    function ($scope, $modal, Person, LicensePlate, Personalroute, Point, Address, Route, AddressFormatter, $http, NotificationService, Token, SmartAdresseSource, $rootScope, $timeout, AppLogin) {
        $scope.gridContainer = {};
        $scope.isCollapsed = true;
        $scope.licenseplates = [];
        $scope.newLicensePlate = "";
        $scope.newLicensePlateDescription = "";
        $scope.workDistanceOverride = 0;
        $scope.routes = [];
        $scope.addresses = [];
        $scope.tokens = [];
        $scope.isCollapsed = true;
        $scope.tokenIsCollapsed = true;
        $scope.newTokenDescription = "";

        $scope.mobileTokenHelpText = $rootScope.HelpTexts.MobileTokenHelpText.text;
        $scope.primaryLicensePlateHelpText = $rootScope.HelpTexts.PrimaryLicensePlateHelpText.text;
        $scope.AlternativeHomeAddressHelpText = $rootScope.HelpTexts.AlternativeHomeAddressHelpText.text;


        var personId = $rootScope.CurrentUser.Id;
        $scope.currentPerson = $rootScope.CurrentUser;
        $scope.mailAdvice = $scope.currentPerson.RecieveMail;

        $scope.showMailNotification = $rootScope.CurrentUser.IsLeader || $rootScope.CurrentUser.IsSubstitute;

        // Used for alternative address template
        $scope.employments = $rootScope.CurrentUser.Employments;

        // Contains references to kendo ui grids.
        $scope.gridContainer = {};





        //Load licenseplates
        $scope.licenseplates = $rootScope.CurrentUser.LicensePlates;

        //Funtionalitet til opslag af adresser
        $scope.SmartAddress = SmartAdresseSource;



        $scope.saveNewLicensePlate = function () {
            /// <summary>
            /// Handles saving of new license plate.
            /// </summary>
            var plateWithoutSpaces = $scope.newLicensePlate.replace(/ /g, "");
            if (plateWithoutSpaces.length < 2 || plateWithoutSpaces.length > 7) {
                NotificationService.AutoFadeNotification("danger", "", "Nummerpladens længde skal være mellem 2 og 7 tegn (Mellemrum tæller ikke med)");
                return;
            }

            var newPlate = new LicensePlate({
                Plate: $scope.newLicensePlate,
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

                NotificationService.AutoFadeNotification("success", "", "Ny nummerplade blev gemt");

                // Reload CurrentUser to update LicensePlates in DrivingController
                Person.GetCurrentUser().$promise.then(function (data) {
                    $rootScope.CurrentUser = data;
                });

            }, function () {
                NotificationService.AutoFadeNotification("danger", "", "Nummerplade blev ikke gemt");
            });
        };

        $scope.openConfirmDeleteLicenseModal = function (plate) {
            /// <summary>
            /// Opens confirm delete license modal.
            /// </summary>
            /// <param name="token"></param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/ConfirmDeleteLicenseModal.html',
                controller: 'AppLoginModalController',
                backdrop: 'static',
            });

            modalInstance.result.then(function () {
                $scope.deleteLicensePlate(plate);
            }, function () {

            });
        };


        $scope.deleteLicensePlate = function (plate) {
            /// <summary>
            /// Delete existing license plate.
            /// </summary>
            /// <param name="plate"></param>
            LicensePlate.delete({ id: plate.Id }, function () {
                NotificationService.AutoFadeNotification("success", "", "Nummerplade blev slettet");
                //Load licenseplates again
                LicensePlate.get({ id: personId }, function (data) {
                    $scope.licenseplates = data;
                });
                // Reload CurrentUser to update LicensePlates in DrivingController
                Person.GetCurrentUser().$promise.then(function (data) {
                    $rootScope.CurrentUser = data;
                });
            }), function () {
                NotificationService.AutoFadeNotification("danger", "", "Nummerplade blev ikke slettet");
            };
        }


        $scope.setReceiveMail = function (receiveMails) {
            /// <summary>
            /// Inverts choice of mail notification.
            /// </summary>

            $timeout(function () {
                var newPerson = new Person({
                    RecieveMail: receiveMails
                });

                newPerson.$patch({ id: personId }, function () {
                    $scope.mailAdvice = receiveMails;
                    $rootScope.CurrentUser.RecieveMail = receiveMails;
                    NotificationService.AutoFadeNotification("success", "", "Valg om modtagelse af mails blev gemt");
                }), function () {

                    NotificationService.AutoFadeNotification("danger", "", "Valg om modtagelse af mails blev ikke gemt");
                };
            });


        }

        $scope.loadGrids = function (id) {
            /// <summary>
            /// Loads personal routes and addresses to kendo grid datasources.
            /// </summary>
            /// <param name="id"></param>
            $scope.personalRoutes = {
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend: function(req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/PersonalRoutes()?$filter=PersonId eq " + id + "&$expand=Points",
                            dataType: "json",
                            cache: false
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
                    pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
                },
                dataBound: function() {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "Description",
                        title: "Beskrivelse"
                    },
                    {
                        field: "Points",
                        template: function(data) {
                            var temp = [];

                            angular.forEach(data.Points, function(value, key) {
                                if (value.PreviousPointId == undefined) {
                                    this.push(value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town);
                                }

                            }, temp);

                            return temp;
                        },
                        title: "Fra"
                    },
                    {
                        field: "Points",
                        template: function(data) {
                            var temp = [];

                            angular.forEach(data.Points, function(value, key) {
                                if (value.NextPointId == null) {
                                    this.push(value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town);
                                }

                            }, temp);

                            return temp;
                        },
                        title: "Til"
                    },
                    {
                        title: "Via",
                        field: "Points",
                        width: 50,
                        template: function(data) {
                            var tooltipContent = "";
                            var gridContent = data.Points.length - 2;
                            angular.forEach(data.Points, function(point, key) {
                                if (key != 0 && key != data.Points.length - 1) {
                                    tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town + "<br/>";
                                }
                            });

                            var result = "<div kendo-tooltip k-content=\"'" + tooltipContent + "'\">" + gridContent + "</div>";
                            return result;
                        }
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
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend: function(req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/PersonalAddresses()?$filter=PersonId eq " + personId + " and Type ne Core.DomainModel.PersonalAddressType'OldHome'",
                            dataType: "json",
                            cache: false
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
                    pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
                },
                dataBound: function() {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                sort: {
                    field: "Description",
                    dir: "asc"
                },
                columns: [
                    {
                        field: "Description",
                        title: "Beskrivelse",
                        template: function(data) {
                            if (data.Type == "Home") {
                                return "Hjemmeadresse";
                            } else return data.Description;
                        }
                    },
                    {
                        field: "Id",
                        template: function(data) {
                            return (data.StreetName + " " + data.StreetNumber + ", " + data.ZipCode + " " + data.Town);
                        },
                        title: "Adresse"
                    },
                    {
                        field: "Id",
                        title: "Muligheder",
                        template: function(data) {
                            if (data.Type == "Standard") {
                                return "<a ng-click='openAddressEditModal(" + data.Id + ")'>Rediger</a> | <a ng-click='openAddressDeleteModal(" + data.Id + ")'>Slet</a>";
                            }
                            return "";
                        }
                    }
                ]
            };
        }

        $rootScope.$on('PersonalAddressesChanged', function () {
            // Event gets emitted from AlternativeAddressController when the user changes alternative home or work addresses.
            $scope.updatePersonalAddresses();
        });

        $scope.loadGrids($rootScope.CurrentUser.Id);

        $scope.updatePersonalAddresses = function () {
            /// <summary>
            /// Refreshes personal addresses data source.
            /// </summary>
            $scope.gridContainer.personalAddressesGrid.dataSource.transport.options.read.url = "odata/PersonalAddresses()?$filter=PersonId eq " + $scope.currentPerson.Id;
            $scope.gridContainer.personalAddressesGrid.dataSource.read();
            // Reload CurrentUser to update Personal Addresses in DrivingController
            Person.GetCurrentUser().$promise.then(function (data) {
                $rootScope.CurrentUser = data;
            });
        }

        $scope.updatePersonalRoutes = function () {
            /// <summary>
            /// refreshes personal routes data source.
            /// </summary>
            $scope.gridcontainer.personalRoutesGrid.dataSource.transport.options.read.url = "odata/PersonalRoutes()?$filter=PersonId eq " + $scope.currentPerson.Id + "&$expand=Points";
            $scope.gridcontainer.personalRoutesGrid.dataSource.read();
            // Reload CurrentUser to update Personal Routes in DrivingController
            Person.GetCurrentUser().$promise.then(function (data) {
                $rootScope.CurrentUser = data;
            });

        }

        $scope.openRouteEditModal = function (id) {
            /// <summary>
            /// Opens edit route modal.
            /// </summary>
            /// <param name="id"></param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/RouteEditModal.html',
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
            /// <summary>
            /// Opens add route modal.
            /// </summary>
            /// <param name="id"></param>

            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/RouteAddModal.html',
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
            /// <summary>
            /// Opens delete route modal.
            /// </summary>
            /// <param name="id"></param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/RouteDeleteModal.html',
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
            /// <summary>
            ///Opens add personal address modal.
            /// </summary>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/AddressAddModal.html',
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
            /// <summary>
            /// Opens edit personal address modal.
            /// </summary>
            /// <param name="id"></param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/AddressEditModal.html',
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
            /// <summary>
            /// Opens delete personal address modal.
            /// </summary>
            /// <param name="id"></param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/AddressDeleteModal.html',
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

        $scope.openConfirmDeleteAppPasswordModal = function () {
            /// <summary>
            /// Opens confirm delete app login modal.
            /// </summary>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/confirmDeleteAppPasswordModal.html',
                controller: 'AppLoginModalController',
                backdrop: 'static',
            });

            modalInstance.result.then(function () {
                AppLogin.delete({ id: $scope.currentPerson.Id }).$promise.then(function () {
                    $scope.currentPerson.HasAppPassword = false;
                    $rootScope.CurrentUser.HasAppPassword = false;
                    NotificationService.AutoFadeNotification("success", "", "App login blev nulstillet.");
                });
            }, function () {

            });
        };

        $scope.openConfirmCreateAppPasswordModal = function () {
            /// <summary>
            /// Opens confirm create app login modal.
            /// </summary>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/confirmCreateAppLoginModal.html',
                controller: 'AppLoginModalController',
                backdrop: 'static',
            });

            modalInstance.result.then(function (res) {
                var appLogin = {Password: res, UserName: $scope.currentPerson.Initials, PersonId: $scope.currentPerson.Id};
                AppLogin.post(appLogin).$promise.then(function () {
                    $scope.currentPerson.HasAppPassword = true;
                    $rootScope.CurrentUser.HasAppPassword = true;
                    NotificationService.AutoFadeNotification("success", "", "App login blev oprettet.");
                });
            }, function () {

            });
        };

        $scope.makeLicensePlatePrimary = function (plate) {
            /// <summary>
            /// Makes license plate primary.
            /// </summary>
            /// <param name="plate"></param>
            LicensePlate.patch({ id: plate.Id }, { IsPrimary: true }, function () {
                //Load licenseplates when finished request.
                LicensePlate.get({ id: personId }, function (data) {
                    $scope.licenseplates = data;
                });
            });
        }

        $scope.openAlternativeWorkAddressModal = function () {

            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Settings/AlternativeWorkAddressModal.html',
                controller: 'AlternativeWorkAddressModalController',
                backdrop: 'static',
            });

            modalInstance.result.then(function (res) {

            }, function () {

            });
        };

        var checkShouldPrompt = function () {
            /// <summary>
            /// Return true if there are unsaved changes on the page.
            /// </summary>

            if ($scope.newLicensePlate != "" ||
                $scope.newLicensePlateDescription != "") {
                return true;
            }
            return false;
        }

        // Alert the user when navigating away from the page if there are unsaved changes.
        $scope.$on('$stateChangeStart', function (event) {
            if (checkShouldPrompt() === true) {
                var answer = confirm("Du har lavet ændringer på siden, der ikke er gemt. Ønsker du at kassere disse ændringer?");
                if (!answer) {
                    event.preventDefault();
                }
            }
        });

        window.onbeforeunload = function (e) {
            if (checkShouldPrompt() === true) {
                return "Du har lavet ændringer på siden, der ikke er gemt. Ønsker du at kassere disse ændringer?";
            }
        };

        $scope.$on('$destroy', function () {
            /// <summary>
            /// Unregister refresh event handler when leaving the page.
            /// </summary>
            window.onbeforeunload = undefined;
        });
    }
]);