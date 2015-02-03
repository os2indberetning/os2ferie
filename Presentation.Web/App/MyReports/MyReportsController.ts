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
                    type: "json",
                    transport: {
                        read: {
                            url: "/odata/TestReports",
                                         
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