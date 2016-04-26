angular.module("app.drive").controller("OrgUnitController", [
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

        $scope.$on('orgSettingsClicked', function (event, mass) {
            $scope.gridContainer.grid.dataSource.read();
        });

        $scope.OrgUnits = {
            autoBind: false,
            dataSource: {
                type: "odata-v4",
                transport: {
                    read: {
                        beforeSend: function(req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "/odata/OrgUnits",
                        dataType: "json",
                        cache: false
                    }
                },
                schema: {
                    model: {
                        fields: {
                            OrgId: {
                                editable: false
                            },
                            ShortDescription: {
                                editable: false
                            },
                            LongDescription: {
                                editable: false
                            },
                            HasAccessToFourKmRule: {
                                editable: false
                            },
                            DefaultKilometerAllowance: {
                                type: "string"
                            }
                        }
                    }
                },
                pageSize: 20,
                serverPaging: true,
                serverFiltering: true
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
            dataBound: function() {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            editable: true,
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
                    template: function(data) {
                        if (data.HasAccessToFourKmRule) {
                            return "<input type='checkbox' ng-click='rowChecked(" + data.Id + ", false)' checked></input>";
                        } else {
                            return "<input type='checkbox' ng-click='rowChecked(" + data.Id + ", true)'></input>";
                        }
                    }
                },
                {
                    field: "DefaultKilometerAllowance",
                    title: "Standard kilometeropgørelse",
                    editor: kilometerAllowanceDropDownEditor,
                    template: function(data) {
                        if (data.DefaultKilometerAllowance === "Calculated") {
                            return "Beregnet";
                        } else if (data.DefaultKilometerAllowance === "Read") {
                            return "Aflæst";
                        } else if (data.DefaultKilometerAllowance === "CalculatedWithoutExtraDistance") {
                            return "Beregnet uden merkørsel";
                        } else {
                            return "Fejl";
                        }
                    }
                }
            ]
        };

        function kilometerAllowanceDropDownEditor(container, options) {

            var orgId = options.model.Id;

            var allowanceData = [
                { name: "Beregnet", value: "Calculated" },
                { name: "Aflæst", value: "Read" },
                { name: "Beregnet uden merkørsel", value: "CalculatedWithoutExtraDistance" }
            ];

            var dataSource = new kendo.data.DataSource({
                data: allowanceData
            });

            $('<input required data-text-field="name" data-value-field="value" data-bind="value:' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    autoBind: false,
                    dataSource: dataSource,
                    change: function (e) {

                        OrgUnit.patch({ id: orgId }, { "DefaultKilometerAllowance": allowanceData[e.sender.selectedIndex].value }).$promise.then(function () {
                            NotificationService.AutoFadeNotification("success", "", "Opdaterede organisationen");

                            //// Reload CurrentUser to update default kilometerallowance in DrivingController
                            Person.GetCurrentUser().$promise.then(function (data) {
                                $rootScope.CurrentUser = data;
                            });
                        });


                    },
                });

        }

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
