angular.module("application").controller("OrgUnitController", [
    "$scope", "OrgUnit", "NotificationService", "$rootScope", "Person", function ($scope, OrgUnit, NotificationService, $rootScope, Person) {
        $scope.gridContainer = {};

        $scope.checkboxes = [];
        $scope.typeAheadOrgUnits = [];
        $scope.orgUnit = {};
        $scope.people = [];

        $scope.autoCompleteOptions = {
            filter: "contains"
        };

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
                        var res = "<input type='checkbox' ng-model='checkboxes[" + data.Id + "]' ng-change='rowChecked(" + data.Id + ")'></input>";
                        return res;
                    }
                }
            ]
        };

        $scope.rowChecked = function (id) {
            /// <summary>
            /// Is called when the user checks an orgunit in the grid.
            /// Patches HasAccessToFourKmRule on the backend.
            /// </summary>
            /// <param name="id"></param>
            var org = "";
            for (var i = 0; i < $scope.typeAheadOrgUnits.length; i++) {
                if ($scope.typeAheadOrgUnits[i].Id == id) {
                    org = $scope.typeAheadOrgUnits[i].LongDescription;
                }
            }

            if ($scope.checkboxes[id]) {
                // Checkbox has been checked.
                
                Enumerable.From($rootScope.OrgUnits).Single(function (x) { return x.Id == id }).HasAccessToFourKmRule = true;

                OrgUnit.patch({ id: id }, { "HasAccessToFourKmRule": true }).$promise.then(function () {
                    NotificationService.AutoFadeNotification("success", "", "Adgang til 4 km-regel givet til " + org);

                    //// Reload CurrentUser to update FourKmRule in DrivingController
                    Person.GetCurrentUser().$promise.then(function (data) {
                        $rootScope.CurrentUser = data;
                    });
                });
            } else if (!$scope.checkboxes[id]) {
                // Checkbox has been unchecked.

                Enumerable.From($rootScope.OrgUnits).Single(function (x) { return x.Id == id }).HasAccessToFourKmRule = false;

                OrgUnit.patch({ id: id }, { "HasAccessToFourKmRule": false }).$promise.then(function () {
                    NotificationService.AutoFadeNotification("success", "", "Adgang til 4 km-regel fjernet fra " + org);

                    //// Reload CurrentUser to update FourKmRule in DrivingController
                    Person.GetCurrentUser().$promise.then(function (data) {
                        $rootScope.CurrentUser = data;
                    });
                });
            }
        }

        angular.forEach($rootScope.OrgUnits, function (org, key) {
            $scope.typeAheadOrgUnits.push({ Id: org.Id, LongDescription: org.LongDescription });
            $scope.checkboxes[org.Id] = org.HasAccessToFourKmRule;
        });

        $scope.orgUnitChanged = function (item) {
            /// <summary>
            /// Filters grid content
            /// </summary>
            /// <param name="item"></param>
            var filter = [];
            filter.push({ field: "LongDescription", operator: "startswith", value: $scope.orgUnit.chosenUnit });
            $scope.gridContainer.grid.dataSource.filter(filter);
        }

        $scope.clearClicked = function () {
            /// <summary>
            /// Clears filters.
            /// </summary>
            $scope.orgUnit.chosenUnit = "";
            $scope.gridContainer.grid.dataSource.filter({});

        }

    }
]);
