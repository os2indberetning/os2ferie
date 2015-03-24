angular.module("application").controller("SubstituteController", [
    "$scope", "$modal", "NotificationService", function ($scope, $modal, Rate, NotificationService, RateType) {


        $scope.container = {};

        $scope.container.subGridPageSize = 20;
        $scope.container.appGridPageSize = 20;

        $scope.pageSizeChanged = function () {
            $scope.container.approverGrid.dataSource.pageSize(Number($scope.container.appGridPageSize));
            $scope.container.substituteGrid.dataSource.pageSize(Number($scope.container.subGridPageSize));
        }


        $scope.substitutes = {
            dataSource: {
                pageSize: 20,
                type: "odata",
                transport: {
                    read: {
                        beforeSend: function (req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "odata/Substitutes?$expand=Leader,Sub,OrgUnit, Person",//"?$expand=OrgUnit and Sub",
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
                        var resultSet = [];
                        angular.forEach(data.value, function (value, key) {
                            // If the PersonId in Substitutes is equal to the LeaderId in Substitutes then it is a substitute.
                            if (value.PersonId == value.LeaderId) {
                                resultSet.push(value);
                            }
                        });

                       
                        return resultSet;

                    },
                    total: function (data) {
                        return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                    }
                },
            },
            sortable: true,
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
                pageSizes: [5, 10, 20, 30, 40, 50]
            },
            dataBound: function () {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            columns: [{
                field: "Sub.FullName",
                title: "Stedfortræder"
            }, {
                field: "Leader.FullName",
                title: "Afviger"
            }, {
                field: "StartDateTimestamp",
                title: "Fra",
                template: function(data) {
                    var m = moment.unix(data.StartDateTimestamp);
                    return m._d.getDate() + "/" +
                        (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                        m._d.getFullYear();
                }
            }, {
                field: "EndDateTimestamp",
                title: "Til",
                template: function (data) {
                    var m = moment.unix(data.EndDateTimestamp);
                    return m._d.getDate() + "/" +
                        (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                        m._d.getFullYear();
                }
            }]
        };

       
        $scope.personalApprovers = {
            dataSource: {
                pageSize: 20,
                type: "odata",
                transport: {
                    read: {
                        beforeSend: function (req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "odata/Substitutes?$expand=Leader,Sub,OrgUnit, Person",
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
                        var resultSet = [];
                        angular.forEach(data.value, function (value, key) {
                            // If the PersonId in Substitutes is not equal to the LeaderId in Substitutes then it is a personal approver.
                            if (value.PersonId != value.LeaderId) {
                                resultSet.push(value);
                            }
                        });


                        return resultSet;

                    },
                    total: function (data) {
                        return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                    }
                },
            },
            sortable: true,
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
                pageSizes: [5, 10, 20, 30, 40, 50]
            },
            dataBound: function () {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            columns: [{
                field: "Sub.FullName",
                title: "Godkender"
            }, {
                field: "Leader.FullName",
                title: "Udpeget af"
            }, {
                field: "Person.FullName",
                title: "Ansat",
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
                    var m = moment.unix(data.EndDateTimestamp);
                    return m._d.getDate() + "/" +
                        (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                        m._d.getFullYear();
                }
            }]
        };

    

     

     

        $scope.gridPageSize = 5;


      
       


     


     

    }
]);
