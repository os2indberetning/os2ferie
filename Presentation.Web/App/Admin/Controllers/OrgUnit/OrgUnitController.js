angular.module("application").controller("OrgUnitController", [
    "$scope", "OrgUnit", function ($scope, OrgUnit) {
        $scope.gridContainer = {};

        $scope.checkboxes = [];
        $scope.typeAheadOrgUnits = [];
        $scope.orgUnit = {};
        $scope.people = [];

        $scope.loadOrgUnits = function () {
            $scope.OrgUnits = {
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
                    pageSizes: [5, 10, 20, 30, 40, 50]
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
                        template: function(data) {
                            var res = "<input type='checkbox' ng-model='checkboxes[" + data.Id + "]' ng-change='rowChecked(" + data.Id + ")'></input>";
                            return res;
                        }
                    }
                ]
            };
        }

        $scope.rowChecked = function (id) {
            if ($scope.checkboxes[id]) {
                // Checkbox has been checked.
                OrgUnit.patch({ id: id }, { "HasAccessToFourKmRule": true });
            } else if(!$scope.checkboxes[id]) {
                // Checkbox has been unchecked.
                OrgUnit.patch({ id: id }, { "HasAccessToFourKmRule": false });
            }
        }

        OrgUnit.get().$promise.then(function (res) {
            angular.forEach(res.value, function (org, key) {
                $scope.typeAheadOrgUnits.push({ Id: org.Id, LongDescription: org.LongDescription });
                $scope.checkboxes[org.Id] = org.HasAccessToFourKmRule;
            });
            $scope.loadOrgUnits();
        });

        $scope.orgUnitChanged = function (item) {
            var filter = [];
            filter.push({ field: "LongDescription", operator: "contains", value: $scope.orgUnit.chosenUnit });
            $scope.gridContainer.grid.dataSource.filter(filter);
        }

        $scope.clearClicked = function() {
            $scope.orgUnit.chosenUnit = "";
            $scope.gridContainer.grid.dataSource.filter({});

        }
       
        $scope.gridContainer.gridPageSize = 20;
       

        $scope.pageSizeChanged = function () {
            $scope.gridContainer.grid.dataSource.pageSize(Number($scope.gridContainer.gridPageSize));
        }
       
    }
]);
