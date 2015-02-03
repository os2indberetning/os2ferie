var app = Application.AngularApp.Module;

app.controller("MyReportsControllerJSTest", function ($scope) {
    $scope.pendingReports = {
        dataSource: {
            type: "odata",
            transport: {
                read: {
                    url: "/odata/TestReports",
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    var paramMap = kendo.data.transports.odata.parameterMap(options);

                    delete paramMap.$inlinecount; // <-- remove inlinecount parameter
                    delete paramMap.$format; // <-- remove format parameter

                    return paramMap;
                },
            },

            schema: {
                data: function (data) {
                    return data; // <-- The result is just the data, it doesn't need to be unpacked.
                },
                total: function (data) {
                    return data.length; // <-- The total items count is the data length, there is no .Count to unpack.

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
                field: "Name"
            }, {
                field: "ReportedDate"
            }, {
                field: "Purpose"
            }, {
                field: "Type"
            }, {
                field: "Title"
            }
        ]


    };
    console.log($scope.pendingReports);
})