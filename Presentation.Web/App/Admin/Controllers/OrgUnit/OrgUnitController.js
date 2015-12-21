angular.module("application").controller("OrgUnitController", [
    "$scope", "OrgUnit", "NotificationService", "$rootScope", "Person", "Autocomplete", function ($scope, OrgUnit, NotificationService, $rootScope, Person, Autocomplete) {
        $scope.gridContainer = {};

        $scope.orgUnits = Autocomplete.orgUnits();
        $scope.orgUnit = {};
        $scope.selectedKmRule = -1;

        $scope.updateSourceUrl = function() {
            var url = "/odata/OrgUnits";

            if (Object.keys($scope.orgUnit).length !== 0) {
                url += "?$filter=contains(LongDescription," + "'" + encodeURIComponent($scope.orgUnit.chosenUnit + "')");
                if ($scope.selectedKmRule !== -1)
                    url += " and HasAccessToFourKmRule eq " + ($scope.selectedKmRule === 0 ? "false" : "true");
            } else {
                if ($scope.selectedKmRule !== -1)
                    url += "?$filter=HasAccessToFourKmRule eq " + ($scope.selectedKmRule === 0 ? "false" : "true");
            }


            $scope.gridContainer.grid.dataSource.transport.options.read.url = url;
            $scope.gridContainer.grid.dataSource.read();
        }

        $scope.autoCompleteOptions = {
            filter: "contains",
            select: function (e) {
                $scope.orgUnit.chosenId = this.dataItem(e.item.index()).Id;
            }
        }

        

        $scope.kmRuleOptions = {
            dataSource: {
                data: [{
                    value: -1,
                    text: 'Alle'
                }, {
                    value: 1,
                    text: 'Bruger 4 km-reglen'
                }, {
                    value: 0,
                    text: 'Bruger ikke 4 km-reglen'
                }]
            },
            dataTextField: 'text',
            dataValueField: 'value',
            select: function(e) {
                var value = this.dataItem(e.item).value;
                $scope.selectedKmRule = value;

                $scope.updateSourceUrl();
            },
            clear: function() {
                this.dataSource.read();
            }
        }

        $scope.$on('4kmClicked', function (event, mass) {
            $scope.gridContainer.grid.dataSource.read();
        });

        $scope.OrgUnits = {
            autoBind: false,
            dataSource: {
                type: "odata-v4",
                transport: {
                    read: {
                        beforeSend: function (req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "/odata/OrgUnits",
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
                serverFiltering: true,
            },
            sortable: true,

            pageable: {
                messages: {
                    display: "{0} - {1} af {2} organisationsenheder", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                    empty: "Ingen organisationsenheder at vise",
                    page: "Side",
                    of: "af {0}", //{0} is total amount of pages
                    itemsPerPage: "Organisationsenheder pr. side",
                    first: "Gå til første side",
                    previous: "Gå til forrige side",
                    next: "Gå til næste side",
                    last: "Gå til sidste side",
                    refresh: "Genopfrisk"
                },
                pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
            },
            dataBound: function () {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            columns: [
                {
                    field: "OrgId",
                    title: "Organisations ID"
                },
                {
                    field: "ShortDescription",
                    title: "Kort beskrivelse"
                },
                {
                    field: "LongDescription",
                    title: "Lang beskrivelse"
                },
                {
                    field: "HasAccessToFourKmRule",
                    title: "Kan benytte 4 km-regel",
                    template: function (data) {
                        if (data.HasAccessToFourKmRule) {
                            return "<input type='checkbox' ng-click='rowChecked(" + data.Id + ", false)' checked></input>";
                        } else {
                            return "<input type='checkbox' ng-click='rowChecked(" + data.Id + ", true)'></input>";
                        }
                    }
                }
            ]
        };

        $scope.rowChecked = function (id, newValue) {
            /// <summary>
            /// Is called when the user checks an orgunit in the grid.
            /// Patches HasAccessToFourKmRule on the backend.
            /// </summary>
            /// <param name="id"></param>

            OrgUnit.patch({ id: id }, { "HasAccessToFourKmRule": newValue }).$promise.then(function () {
                if (newValue) {
                    NotificationService.AutoFadeNotification("success", "", "Adgang til 4 km-regel tilføjet.");
                } else {
                    NotificationService.AutoFadeNotification("success", "", "Adgang til 4 km-regel fjernet.");
                }


                //// Reload CurrentUser to update FourKmRule in DrivingController
                Person.GetCurrentUser().$promise.then(function (data) {
                    $rootScope.CurrentUser = data;
                });
            });
        }

        $scope.orgUnitChanged = function (item) {
            /// <summary>
            /// Filters grid content
            /// </summary>
            /// <param name="item"></param>
            $scope.updateSourceUrl();
        }

        $scope.clearClicked = function () {
            /// <summary>
            /// Clears filters.
            /// </summary>
            $scope.selectedKmRule = -1;
            $scope.gridContainer.kmRule.select(0);
            $scope.orgUnit.chosenUnit = "";
            $scope.gridContainer.grid.dataSource.transport.options.read.url = "/odata/OrgUnits";
            $scope.gridContainer.grid.dataSource.read();
        }

    }
]);
