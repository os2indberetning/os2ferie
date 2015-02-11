angular.module("application").controller("SettingController", [
    "$scope", "Person", "LicensePlate", "Personalroute", "Point", "$http", function ($scope, Person, LicensePlate, Personalroute, Point, $http) {
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
                            url: "odata/Points(" + id + ")",
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
                        field: "Id",
                        title: "Navn"
                    }, {
                        field: "Description",
                        title: "Indberettet den"
                    }, {
                        field: "Plate",
                        title: "Formål"
                    }, {
                        field: "Type",
                        title: "Type"
                    }, {
                        field: "options",
                        title: "Muligheder"
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
                        field: "Id",
                        title: "Navn"
                    }, {
                        field: "Description",
                        title: "Indberettet den"
                    }, {
                        field: "Plate",
                        title: "Formål"
                    }, {
                        field: "Type",
                        title: "Type"
                    }, {
                        field: "options",
                        title: "Muligheder"
                    }
                ]
            };

            
        }


        $scope.loadPersonalRoutes = function (id) {
            
        }

        $scope.GetPerson = Person.get({ id: 1 }, function (data) {
            $scope.currentPerson = data[0];

        },
        function () {

        });

        //$scope.loadPersonalRoutes(1);
        //$scope.loadPersonalAddresses(1);
        $scope.loadGrids(1);


    }
]);

angular.module("application").controller('TokenController', ["$scope", "$modal", function ($scope, $modal) {

    $scope.items = ['Nr. 1: 123456', 'Nr. 1: 234567', 'Nr. 1: 345678'];

    $scope.openTokenModal = function (size) {

        var modalInstance = $modal.open({
            templateUrl: '/App/Settings/tokenModal.html',
            controller: 'TokenInstanceController',
            size: size,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        });
    };
}]);

angular.module("application").controller('TokenInstanceController', ["$scope", "$modalInstance", "items", function ($scope, $modalInstance, items) {

    $scope.items = items;
    $scope.selected = {
        item: $scope.items[0]
    };

    $scope.closeTokenModal = function () {
        $modalInstance.dismiss('Luk');
    };
}]);