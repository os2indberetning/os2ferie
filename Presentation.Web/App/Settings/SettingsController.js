angular.module("application").controller("SettingController", [
    "$scope", "Person", "LicensePlate", "Personalroute", "Point", "RouteContainer", "$http", function ($scope, Person, LicensePlate, Personalroute, Point, RouteContainer, $http) {
        $scope.isCollapsed = true;
        $scope.mailAdvice = 'No';
        $scope.licenseplates = [];

        // Contains references to kendo ui grids.
        $scope.gridContainer = {};

        $scope.setHomeWorkOverride = function () {
            $http({ method: 'PATCH', url: "odata/Person(1)", data: { workDistanceOverride: 42 } })
                .success();
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
                            url: "odata/PersonalRoutes(" + id + ")?$expand=Points",
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
                    pageSize: 5,
                    serverPaging: true,
                    serverSorting: true
                },
                sortable: true,
                pageable: true,
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
                        title: "Adresse 1"
                    }, {
                        field: "Id",
                        template: function (data) {
                            var temp = [];
                            
                            angular.forEach(data.Points, function (value, key) {
                                if (value.NextPointId == undefined) {
                                    this.push(value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town);
                                }

                            }, temp);

                            return temp;
                        },
                        title: "Adresse 2"
                    },
                    {
                        field: "Id",
                        title: "Muligheder",
                        template: "<a ng-click='editRoute(${Id})'>Rediger</a>"
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
                            url: "odata/PersonalAddresses(" + id + ")",
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
                    pageSize: 5,
                    serverPaging: true,
                    serverSorting: true
                },
                sortable: true,
                pageable: true,
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "Description",
                        title: "Beskrivelse"
                    }, {
                        field: "Id",
                        template: function(data) {
                            return (data.StreetName + " " + data.StreetNumber + ", " + data.ZipCode + " " + data.Town);
                        },
                        title: "Indberettet den"
                    }, {
                        field: "Id",
                        title: "Muligheder",
                        template: "<a ng-click=editAddress(${Id})>Rediger</a>"
                    }
                ]
            };
        }

        $scope.GetPerson = Person.get({ id: 1 }, function (data) {
            $scope.currentPerson = data.value[0];
                
            },
        function () {

        });

        $scope.loadGrids(1);

        $scope.editRoute = function(id) {
            
        }

        $scope.editAddress = function (id) {
            
        }
    }
]);