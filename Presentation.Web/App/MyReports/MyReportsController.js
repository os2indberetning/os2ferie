angular.module("application").controller("MyReportsController", [
   "$scope", "$modal", "$rootScope", "Report", function ($scope, $modal, $rootScope, Report) {



// Helper Methods

       $scope.updateActiveTab = function(query) {
           if ($scope.activeTab == 'pending') {
               // Update pending tabs.
               this.updatePendingReports(query);
           }
           else if ($scope.activeTab == 'approved') {
               // Update approved reports grid
               this.updateApprovedReports(query);
           }
           else if ($scope.activeTab == 'denied') {
               // Update denied reports grid.
               this.updateDeniedReports(query);
           }
       }

       $scope.updatePendingReports = function (oDataQuery) {

           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=status eq Core.DomainModel.ReportStatus'Pending' " + and + oDataQuery;
           $scope.gridContainer.pendingGrid.dataSource.read();
       }

       $scope.updateApprovedReports = function (oDataQuery) {

           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.approvedGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=status eq Core.DomainModel.ReportStatus'Accepted' " + and + oDataQuery;
           $scope.gridContainer.approvedGrid.dataSource.read();
       }

       $scope.updateDeniedReports = function (oDataQuery) {
           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.deniedGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=status eq Core.DomainModel.ReportStatus'Rejected' " + and + oDataQuery;
           $scope.gridContainer.deniedGrid.dataSource.read();
       }

       $scope.loadPendingReports = function () {
           $scope.pendingReports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/DriveReports?$filter=status eq Core.DomainModel.ReportStatus'Pending'",
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
                       field: "CreationDate",
                       template: function (data) {
                           var m = moment.unix(data.CreatedDateTimestamp);
                           return m._d.getDate() + "/" +
                                 (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                  m._d.getFullYear();
                       },
                       title: "Indberettet den"
                   }, {
                       field: "DriveDateTimestamp",
                       template: function(data) {
                           var m = moment.unix(data.DriveDateTimestamp);
                           return m._d.getDate() + "/" +
                               (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                               m._d.getFullYear();
                       },
                       title: "Kørselsdato"
                   }, {
                       field: "Purpose",
                       title: "Formål"
                   }, {
                       field: "Type",
                       title: "Type"
                   }, {
                       field: "Id",
                       template: "<a ng-click=deleteClick(${Id})>Slet</a> | <a ng-click=editClick(${Id})>Rediger</a>",
                       title: "Muligheder"
                   }
               ]
           };
       }

       $scope.loadApprovedReports = function () {
           $scope.approvedReports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/DriveReports?$filter=status eq Core.DomainModel.ReportStatus'Accepted'",
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
                       field: "CreationDate",
                       template: function (data) {
                           var m = moment.unix(data.CreatedDateTimestamp);
                           return m._d.getDate() + "/" +
                                 (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                  m._d.getFullYear();
                       },
                       title: "Indberettet den"
                   }, {
                       field: "DriveDateTimestamp",
                       template: function (data) {
                           var m = moment.unix(data.DriveDateTimestamp);
                           return m._d.getDate() + "/" +
                               (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                               m._d.getFullYear();
                       },
                       title: "Kørselsdato"
                   }, {
                       field: "Purpose",
                       title: "Formål"
                   }, {
                       field: "Type",
                       title: "Type"
                   }
               ]
           };
       }

       $scope.loadDeniedReports = function () {
           $scope.deniedReports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/DriveReports?$filter=status eq Core.DomainModel.ReportStatus'Rejected'",
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
                       field: "CreationDate",
                       template: function (data) {
                           var m = moment.unix(data.CreatedDateTimestamp);
                           return m._d.getDate() + "/" +
                                 (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                  m._d.getFullYear();
                       },
                       title: "Indberettet den"
                   }, {
                       field: "DriveDateTimestamp",
                       template: function (data) {
                           var m = moment.unix(data.DriveDateTimestamp);
                           return m._d.getDate() + "/" +
                               (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                               m._d.getFullYear();
                       },
                       title: "Kørselsdato"
                   }, {
                       field: "Purpose",
                       title: "Formål"
                   }, {
                       field: "Type",
                       title: "Type"
                   }
               ]
           };
       }

       

       $scope.getEndOfDayStamp = function(d){
            var m = moment(d);
            return m.endOf('day').unix();
       }

       $scope.getStartOfDayStamp = function (d) {
           var m = moment(d);
           return m.startOf('day').unix();
       }

// Event handlers

       $scope.clearClicked = function () {
           $scope.toDate = "";
           $scope.fromDate = "";
           $scope.updateActiveTab("");
       }

       $scope.tabClicked = function (tab) {
           $scope.activeTab = tab;
       }

       $scope.searchClicked = function () {
           console.log("called");
           console.log($scope.fromDate);
           console.log($scope.toDate);
           console.log("--------");
           // Validate input
           if (!(typeof $scope.fromDate == 'undefined' || typeof $scope.toDate == 'undefined'
               || $scope.fromDate == "" || $scope.toDate == "")) {

               // Input is valid
               var query = "CreatedDateTimestamp ge " + $scope.getStartOfDayStamp($scope.fromDate) + " and CreatedDateTimestamp le " + $scope.getEndOfDayStamp($scope.toDate);
               $scope.updateActiveTab(query);
           }
       }

       $scope.deleteClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/MyReports/ConfirmDeleteTemplate.html',
               controller: 'ConfirmDeleteReportController',
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function (itemId) {
               // Handle confirm delete
           });
       }

       $scope.editClick = function (id) {
           // Create a new scope to inject into DrivingController
           var scope = $rootScope.$new();

           // Get the report from the server
           Report.get({ id: id }, function (data) {
               // Set values in the scope.
               scope.purpose = data.purpose;
               scope.driveDate = moment().unix(data.driveDateTimestamp).toString();
           });




           var modalInstance = $modal.open({
               scope: scope,
               templateUrl: '/App/MyReports/EditReportTemplate.html',
               controller: 'DrivingController',
               windowClass: 'full',
               resolve: {
                   itemId: function () {
                       return "hej";
                   }
               }
           });

           modalInstance.result.then(function (itemId) {

           });
       }


// Init

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

       // Load up the grids.
       $scope.loadApprovedReports();
       $scope.loadDeniedReports();
       $scope.loadPendingReports();

    }
]);