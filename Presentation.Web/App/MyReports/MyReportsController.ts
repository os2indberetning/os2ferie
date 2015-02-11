/// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />

module MyReports {
    'use strict';
    interface Scope extends ng.IScope {
        pendingReports: any;
        approvedReports: any;
        deniedReports: any;
        searchClicked: any;
        toDate: string;
        fromDate: string;
        gridContainer: any;
        dateOptions : any;
    }

    export class Controller {


        constructor(private $scope: Scope) {


            $scope.fromDate = new Date().toString();



            $scope.dateOptions = {
                format: "dd-MM-yyyy"
            };

            $scope.gridContainer = {};

            $scope.searchClicked = this.searchClicked;




            this.loadPendingReports();
            this.loadApprovedReports();
            this.loadDeniedReports();
        }

         // Eventhandler for searchbutton click
        searchClicked = () => {
                // Validate input
                if (!(typeof this.$scope.fromDate == 'undefined'
                    || typeof this.$scope.toDate == 'undefined'
                    || this.$scope.fromDate == ""
                    || this.$scope.toDate == ""
                    )) {
                    // Input is valid
                    var query = "?$filter=CreatedDateTimestamp ge " + moment(this.$scope.fromDate).unix() + " and CreatedDateTimestamp le " + moment(this.$scope.toDate).unix();
                    this.updatePendingReports(query);
                }
            }

        updatePendingReports = (oDataQuery : string) => {
            this.$scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = "/odata/DriveReports" + oDataQuery;
            this.$scope.gridContainer.pendingGrid.dataSource.read();
        }

        updateApprovedReports = (oDataQuery: string) => {
            this.$scope.gridContainer.approvedGrid.dataSource.transport.options.read.url = "/odata/DriveReports" + oDataQuery;
            this.$scope.gridContainer.approvedGrid.dataSource.read();
        }

        updateDeniedReports = (oDataQuery: string) => {
            this.$scope.gridContainer.deniedGrid.dataSource.transport.options.read.url = "/odata/DriveReports" + oDataQuery;
            this.$scope.gridContainer.deniedGrid.dataSource.read();
        }

        loadPendingReports = () => {
            this.$scope.pendingReports = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend: function(req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "/odata/DriveReports",
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
                        field: "Fullname",
                        title: "Navn"
                    }, {
                        field: "Timestamp",
                        title: "Indberettet den"
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
        }

        loadApprovedReports = () => {
            this.$scope.approvedReports = {
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
        }

        loadDeniedReports = () => {
            this.$scope.deniedReports = {
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
        }



        dateToEpoch = (date: string) : number => {
            var myDate = new Date(date);
            myDate.setHours(myDate.getHours() + 1); // Add 1 hour to get Danish timezone.
            return myDate.getTime() / 1000.0; // return epoch
        }

    }
}

Application.AngularApp.Module.controller("MyReportsController", MyReports.Controller);