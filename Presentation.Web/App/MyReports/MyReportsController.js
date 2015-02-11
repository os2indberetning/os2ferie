angular.module("application").controller("MyReportsController", [
   "$scope", function ($scope) {

       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
     

       // Set initial values for kendo datepickers.
       $scope.toDate = new Date();
       $scope.fromDate = new Date();

       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",
       };

       // Set activeTab's initial value to pending.
       $scope.activeTab = "pending";



       // Searchbutton click event handler
       $scope.searchClicked = function () {

           // Validate input
           if (!(typeof $scope.fromDate == 'undefined' || typeof $scope.toDate == 'undefined'
               || $scope.fromDate == "" || $scope.toDate == "")) {
               
               // Input is valid
               if ($scope.activeTab == 'pending') {
                   // Update pending tabs.
                   var query = "?$filter=CreatedDateTimestamp ge " + moment($scope.fromDate).unix() + " and CreatedDateTimestamp le " + moment($scope.toDate).unix();
                   this.updatePendingReports(query);
               }
               else if ($scope.activeTab == 'approved') {
                   // Update approved reports grid
               }
               else if ($scope.activeTab == 'denied') {
                   // Update denied reports grid.
               }
            }
        }

       // Update pending reports grid based on input odataquery
       $scope.updatePendingReports = function (oDataQuery) {
           $scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = "/odata/DriveReports" + oDataQuery;
           $scope.gridContainer.pendingGrid.dataSource.read();
       }

       // Update approved reports grid based on input odataquery
       $scope.updateApprovedReports = function (oDataQuery) {
           $scope.gridContainer.approvedGrid.dataSource.transport.options.read.url = "/odata/DriveReports" + oDataQuery;
           $scope.gridContainer.approvedGrid.dataSource.read();
       }

       // Update denied reports grid based on input odataquery
       $scope.updateDeniedReports = function (oDataQuery) {
           $scope.gridContainer.deniedGrid.dataSource.transport.options.read.url = "/odata/DriveReports" + oDataQuery;
           $scope.gridContainer.deniedGrid.dataSource.read();
       }


       // Event handler for tab click.
       $scope.tabClicked = function(tab) {
           $scope.activeTab = tab;
       }


       // Load all pending reports from server.
       $scope.loadPendingReports = function () {
           $scope.pendingReports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
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

       // Load all approved reports from server.
       $scope.loadApprovedReports = function () {
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
       }

       // Load all denied reports from server.
       $scope.loadDeniedReports = function () {
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
       }


       // Load up the grids.
       $scope.loadApprovedReports();
       $scope.loadDeniedReports();
       $scope.loadPendingReports();

   }
]);