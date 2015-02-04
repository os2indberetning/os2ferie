/// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />

module MyReports {
    'use strict';
    export interface Scope extends ng.IScope {
        pendingReports: any;
        approvedReports: any;
        deniedReports: any;
    }

    export class Controller {

        constructor(private $scope: Scope) {
            $scope.pendingReports = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend: function (req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "/odata/TestReports",
                            dataType: "json"                                    
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
                        field: "Name",
                        title: "Navn"
                    }, {
                        field: "ReportedDate",
                        title: "Indberetet den"
                    }, {
                        field: "Purpose",
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
           
            $scope.approvedReports = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: "http://demos.telerik.com/kendo-ui/service/Northwind.svc/Employees"
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
                        field: "FirstName",
                        title: "First Name",
                        width: "120px"
                    }, {
                        field: "LastName",
                        title: "Last Name",
                        width: "120px"
                    }, {
                        field: "Country",
                        width: "120px"
                    }, {
                        field: "City",
                        width: "120px"
                    }, {
                        field: "Title"
                    }
                ]
            };

            $scope.deniedReports = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: "http://demos.telerik.com/kendo-ui/service/Northwind.svc/Employees"
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
                        field: "FirstName",
                        title: "First Name",
                        width: "120px"
                    }, {
                        field: "LastName",
                        title: "Last Name",
                        width: "120px"
                    }, {
                        field: "Country",
                        width: "120px"
                    }, {
                        field: "City",
                        width: "120px"
                    }, {
                        field: "Title"
                    }
                ]
            };

            console.log(this.$scope);
        }
    }
}

Application.AngularApp.Module.controller("MyReportsController", MyReports.Controller);