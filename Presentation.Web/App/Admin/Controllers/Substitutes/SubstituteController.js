angular.module("application").controller("SubstituteController", [
    "$scope", "$modal", "NotificationService", function ($scope, $modal, Rate, NotificationService, RateType) {




        $scope.substitutes = {
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        beforeSend: function (req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "odata/Substitutes()",//"?$expand=OrgUnit and Sub",
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
                }
            },
            dataBound: function () {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            columns: [{
                field: "Id",
                title: "Stedfortræder"
            }, {
                field: "StartDateTimestamp",
                title: "Fra"
            }, {
                field: "EndDateTimestamp",
                title: "Til"
            }, {
                field: "Id",
                title: "Organisation"
            }, {
                field: "Id",
                title: "Muligheder"
            }]
        };

       
        $scope.personalApprovers = {
            dataSource: {
                type: "odata",
                transport: {
                    read: {
                        beforeSend: function (req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "odata/Substitutes()",
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
                }
            },
            dataBound: function () {
                this.expandRow(this.tbody.find("tr.k-master-row").first());
            },
            columns: [{
                field: "Id",
                title: "Godkender"
            }, {
                field: "Id",
                title: "Afviger"
            }, {
                field: "Id",
                title: "Ansatte"
            }, {
                field: "StartDateTimestamp",
                title: "Fra"
            }, {
                field: "EndDateTimestamp",
                title: "Til"
            }, {
                field: "Title",
                title: "Muligheder"
            }]
        };

    

     

     

        $scope.gridPageSize = 5;


      
       


     


     

    }
]);
