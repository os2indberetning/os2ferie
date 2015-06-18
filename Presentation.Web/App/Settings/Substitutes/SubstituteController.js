angular.module("application").controller("SubstituteController", [
    "$scope", "$rootScope", "$modal", "NotificationService", "$timeout", "Person", "OrgUnit",
    function ($scope, $rootScope, $modal, NotificationService, $timeout, Person, OrgUnit) {

        $scope.container = {};

        var personId = $rootScope.CurrentUser.Id;

        $scope.orgUnits = [];
        $scope.persons = [];
        $scope.currentPerson = {};

        $scope.getEndOfDayStamp = function (d) {
            var m = moment(d);
            return m.endOf('day').unix();
        }

        $scope.getStartOfDayStamp = function (d) {
            var m = moment(d);
            return m.startOf('day').unix();
        }

        // dates for kendo filter.
        var fromDateFilter = new Date();
        fromDateFilter.setDate(fromDateFilter.getDate() - 30);
        fromDateFilter = $scope.getStartOfDayStamp(fromDateFilter);
        var toDateFilter = $scope.getEndOfDayStamp(new Date());

        $scope.currentPerson = $rootScope.CurrentUser;

        //Person.get({ id: personId }, function (data) {
        //    $scope.currentPerson = data;
        //});

        Person.getAll({ query: "$select=Id,FullName" }).$promise.then(function(res) {
            $scope.persons = res.value;
        });

        OrgUnit.get(function (data) {
            $scope.orgUnits = data.value;
        });

        $scope.substitutes = {
            dataSource: {
                pageSize: 20,
                type: "odata",
                transport: {
                    read: {
                        beforeSend: function (req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "odata/Substitutes/Service.Substitute?$expand=OrgUnit,Sub,Person,Leader",
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
            },
            serverPaging: true,
            serverAggregates: false,
            serverSorting: true,
            serverFiltering: true,
            sortable: true,
            scrollable: false,
            filter: [
                    { field: "StartDateTimestamp", operator: "lte", value: toDateFilter },
                    { field: "EndDateTimestamp", operator: "gte", value: fromDateFilter }
            ],
            pageable: {
                messages: {
                    display: "{0} - {1} af {2} stedfortrædere", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                    empty: "Ingen stedfortrædere at vise",
                    page: "Side",
                    of: "af {0}", //{0} is total amount of pages
                    itemsPerPage: "stedfortrædere pr. side",
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
            columns: [{
                field: "Sub.FullName",
                title: "Stedfortræder"
            }, {
                field: "Person.FullName",
                title: "Stedfortræder for"
            }, {
                field: "OrgUnit.LongDescription",
                title: "Organisationsenhed"
            }, {
                field: "Leader.FullName",
                title: "Opsat af"
            },
            {
                field: "StartDateTimestamp",
                title: "Fra",
                template: function (data) {
                    var m = moment.unix(data.StartDateTimestamp);
                    return m._d.getDate() + "/" +
                        (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                        m._d.getFullYear();
                }
            }, {
                field: "EndDateTimestamp",
                title: "Til",
                template: function (data) {
                    if (data.EndDateTimestamp == 9999999999) {
                        return "På ubestemt tid";
                    }
                    var m = moment.unix(data.EndDateTimestamp);
                    return m._d.getDate() + "/" +
                        (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                        m._d.getFullYear();
                }
            },
            {
                title: "Muligheder",
                template: "<a ng-click='openEditSubstitute(${Id})'>Rediger</a> | <a ng-click='openDeleteSubstitute(${Id})'>Slet</a>"
            }]
        };

        $scope.applyDateFilter = function (fromDateStamp, toDateStamp, type) {
            if (type == "substitute") {
                $scope.container.substituteGrid.dataSource.filter([]);
                var subFilters = [];
                subFilters.push({ field: "StartDateTimestamp", operator: "lte", value: toDateStamp });

                if (!$scope.container.subInfinitePeriod) {
                    subFilters.push({ field: "EndDateTimestamp", operator: "gte", value: fromDateStamp });
                }
                $scope.container.substituteGrid.dataSource.filter(subFilters);
            } else if (type == "approver") {
                $scope.container.approverGrid.dataSource.filter([]);
                var appFilters = [];
                appFilters.push({ field: "StartDateTimestamp", operator: "lte", value: toDateStamp });

                if (!$scope.container.appInfinitePeriod) {
                    appFilters.push({ field: "EndDateTimestamp", operator: "gte", value: fromDateStamp });
                }
                $scope.container.approverGrid.dataSource.filter(appFilters);
            }
        }


        $scope.personalApprovers = {
            dataSource: {
                pageSize: 20,
                type: "odata",
                transport: {
                    read: {
                        beforeSend: function (req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "odata/Substitutes/Service.Personal?$expand=OrgUnit,Sub,Leader,Person",
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
            },
            serverPaging: true,
            serverAggregates: false,
            serverSorting: true,
            serverFiltering: true,
            sortable: true,
            scrollable: false,
            pageable: {
                messages: {
                    display: "{0} - {1} af {2} personlige godkendere", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                    empty: "Ingen personlige godkendere at vise",
                    page: "Side",
                    of: "af {0}", //{0} is total amount of pages
                    itemsPerPage: "personlige godkendere pr. side",
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
            columns: [{
                field: "Sub.FullName",
                title: "Godkender"
            }, {
                field: "Person.FullName",
                title: "Godkender for"
            }, {
                field: "Leader.FullName",
                title: "Opsat af",
            }, {
                field: "StartDateTimestamp",
                title: "Fra",
                template: function (data) {
                    var m = moment.unix(data.StartDateTimestamp);
                    return m._d.getDate() + "/" +
                        (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                        m._d.getFullYear();
                }
            }, {
                field: "EndDateTimestamp",
                title: "Til",
                template: function (data) {
                    if (data.EndDateTimestamp == 9999999999) {
                        return "På ubestemt tid";
                    }
                    var m = moment.unix(data.EndDateTimestamp);
                    return m._d.getDate() + "/" +
                        (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                        m._d.getFullYear();
                }
            },
            {
                title: "Muligheder",
                template: "<a ng-click='openEditApprover(${Id})'>Rediger</a> | <a ng-click='openDeleteApprover(${Id})'>Slet</a>"
            }]
        };

        $scope.openDeleteApprover = function (id) {
            var modalInstance = $modal.open({
                templateUrl: 'App/ApproveReports/Modals/ConfirmDeleteApproverModal.html',
                controller: 'ConfirmDeleteApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: function () {
                        return $scope.persons;
                    },
                    orgUnits: function () {
                        return $scope.orgUnits;
                    },
                    leader: function () {
                        return $scope.currentPerson;
                    },
                    substituteId: function () {
                        return id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.container.approverGrid.dataSource.read();
            }, function () {

            });
        }

        $scope.openEditApprover = function (id) {
            var modalInstance = $modal.open({
                templateUrl: 'App/ApproveReports/Modals/editApproverModal.html',
                controller: 'EditApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: function () {
                        return $scope.persons;
                    },
                    orgUnits: function () {
                        return $scope.orgUnits;
                    },
                    leader: function () {
                        return $scope.currentPerson;
                    },
                    substituteId: function () {
                        return id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.container.approverGrid.dataSource.read();
            }, function () {

            });
        }

        $scope.dateChanged = function (type) {
            // $timeout is a bit of a hack, but it is needed to get the current input value because ng-change is called before ng-model updates.
            $timeout(function () {
                if (type == "substitute") {
                    var subFrom = $scope.getStartOfDayStamp($scope.container.substituteFromDate);
                    var subTo = $scope.getEndOfDayStamp($scope.container.substituteToDate);

                    // Initial load is also a bit of a hack.
                    // dateChanged is called twice when the default values for the datepickers are set.
                    // This leads to sorting the grid content on load, which is not what we want.
                    // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
                    if (initialLoad <= 0) {
                        $scope.applyDateFilter(subFrom, subTo, "substitute");
                    }
                } else if (type == "approver") {
                    var from = $scope.getStartOfDayStamp($scope.container.approverFromDate);
                    var to = $scope.getEndOfDayStamp($scope.container.approverToDate);

                    // Initial load is also a bit of a hack.
                    // dateChanged is called twice when the default values for the datepickers are set.
                    // This leads to sorting the grid content on load, which is not what we want.
                    // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
                    if (initialLoad <= 0) {
                        $scope.applyDateFilter(from, to, "approver");
                    }
                }

                initialLoad--;
            }, 0);
        }

        $scope.refreshGrids = function () {
            $scope.container.approverGrid.dataSource.read();
            $scope.container.substituteGrid.dataSource.read();
        }

        $scope.clearClicked = function (type) {
            var from = new Date();
            from.setDate(from.getDate() - 30);

            if (type == "substitute") {
                $scope.container.subInfinitePeriod = false;
                $scope.container.substituteToDate = new Date();
                $scope.container.substituteFromDate = from;
                $scope.container.substituteGrid.dataSource.filter([]);
            }
            else if (type == "approver") {
                $scope.container.appInfinitePeriod = false;
                $scope.container.approverToDate = new Date();
                $scope.container.approverFromDate = from;
                $scope.container.approverGrid.dataSource.filter([]);
            }
        }

        // Format for datepickers.
        $scope.dateOptions = {
            format: "dd/MM/yyyy",

        };

        $scope.loadInitialDates = function () {
            // Set initial values for kendo datepickers.

            initialLoad = 4;

            var from = new Date();
            from.setDate(from.getDate() - 30);

            $scope.container.approverToDate = new Date();
            $scope.container.approverFromDate = from;

            $scope.container.substituteToDate = new Date();
            $scope.container.substituteFromDate = from;
        }

        $scope.createNewApprover = function () {
            var modalInstance = $modal.open({
                templateUrl: 'App/ApproveReports/Modals/newApproverModal.html',
                controller: 'NewApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: function () {
                        return $scope.persons;
                    },
                    orgUnits: function () {
                        return $scope.orgUnits;
                    },
                    leader: function () {
                        return $scope.currentPerson;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.container.approverGrid.dataSource.read();
            }, function () {

            });
        };

        $scope.createNewSubstitute = function () {
            var modalInstance = $modal.open({
                templateUrl: 'App/Admin/HTML/Substitutes/Modals/newSubstituteModal.html',
                controller: 'AdminNewSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    leader: function () {
                        return $scope.currentPerson;
                    },
                    persons: function () {
                        return $scope.persons;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.container.substituteGrid.dataSource.read();
            }, function () {

            });
        };


        $scope.openEditSubstitute = function (id) {
            var modalInstance = $modal.open({
                templateUrl: 'App/ApproveReports/Modals/editSubstituteModal.html',
                controller: 'EditSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: function () {
                        return $scope.persons;
                    },
                    orgUnits: function () {
                        return $scope.orgUnits;
                    },
                    leader: function () {
                        return $scope.currentPerson;
                    },
                    substituteId: function () {
                        return id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.refreshGrids();
            }, function () {

            });
        }

        $scope.openDeleteSubstitute = function (id) {
            var modalInstance = $modal.open({
                templateUrl: 'App/ApproveReports/Modals/ConfirmDeleteSubstituteModal.html',
                controller: 'ConfirmDeleteSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: function () {
                        return $scope.persons;
                    },
                    orgUnits: function () {
                        return $scope.orgUnits;
                    },
                    leader: function () {
                        return $scope.currentPerson;
                    },
                    substituteId: function () {
                        return id;
                    }
                }
            });

            modalInstance.result.then(function () {
                $scope.refreshGrids();
            }, function () {

            });
        }

        $scope.loadInitialDates();
    }
]);
