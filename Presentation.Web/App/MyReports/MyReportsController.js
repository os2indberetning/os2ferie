angular.module("application").controller("MyReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "$timeout", function ($scope, $modal, $rootScope, Report, $timeout) {

       // Hardcoded personid == 4 until we can get current user from their system.
       var personId = 1;

       // Helper Methods

       $scope.updateActiveTab = function (query) {
           if ($scope.activeTab == 'pending') {
               // Update pending tabs.
               this.updatePendingReports(query);
           }
           else if ($scope.activeTab == 'accepted') {
               // Update accepted reports grid
               this.updateAcceptedReports(query);
           }
           else if ($scope.activeTab == 'rejected') {
               // Update rejected reports grid.
               this.updateRejectedReports(query);
           }
       }

       $scope.updatePendingReports = function (oDataQuery) {

           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending' and PersonId eq " + personId + " " + and + oDataQuery;
           $scope.gridContainer.pendingGrid.dataSource.read();
       }

       $scope.updateAcceptedReports = function (oDataQuery) {

           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.acceptedGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted' and PersonId eq " + personId + " " + and + oDataQuery;
           $scope.gridContainer.acceptedGrid.dataSource.read();
       }

       $scope.updateRejectedReports = function (oDataQuery) {
           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.rejectedGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected' and PersonId eq " + personId + " " + and + oDataQuery;
           $scope.gridContainer.rejectedGrid.dataSource.read();
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
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending' and PersonId eq " + personId,
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
                   serverPaging: false,
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
                       field: "Id",
                       template: function (data) {
                           if (data.Comment != "") {
                               return data.Purpose + "<button kendo-tooltip k-position=\"'right'\" k-content=\"'" + data.Comment + "'\" class=\"k-group btn btn-default pull-right no-border\"><i class=\"fa fa-comment-o\"></i></button>";
                           }
                           return data.Purpose;

                       },
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

       $scope.loadAcceptedReports = function () {
           $scope.acceptedReports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted' and PersonId eq " + personId,
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
                   serverPaging: false,
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
                       field: "Id",
                       template: function (data) {
                           if (data.Comment != "") {
                               return data.Purpose + "<button kendo-tooltip k-position=\"'right'\" k-content=\"'" + data.Comment + "'\" class=\"k-group btn btn-default pull-right no-border\"><i class=\"fa fa-comment-o\"></i></button>";
                           }
                           return data.Purpose;

                       },
                       title: "Formål"
                   }, {
                       field: "Type",
                       title: "Type"
                   }
               ]
           };
       }

       $scope.loadRejectedReports = function () {
           $scope.rejectedReports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },



                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected' and PersonId eq " + personId,
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
                   serverPaging: false,
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
                       field: "Id",
                       template: function (data) {
                           if (data.Comment != "") {
                               return data.Purpose + "<button kendo-tooltip k-position=\"'right'\" k-content=\"'" + data.Comment + "'\" class=\"k-group btn btn-default pull-right no-border\"><i class=\"fa fa-comment-o\"></i></button>";
                           }
                           return data.Purpose;

                       },
                       title: "Formål"
                   }, {
                       field: "Type",
                       title: "Type"
                   }
               ]
           };
       }

       $scope.loadInitialDates = function () {
           // Set initial values for kendo datepickers.
           $scope.dateContainer.toDatePending = new Date();
           $scope.dateContainer.fromDatePending = new Date();
           $scope.dateContainer.toDateAccepted = new Date();
           $scope.dateContainer.fromDateAccepted = new Date();
           $scope.dateContainer.toDateRejected = new Date();
           $scope.dateContainer.fromDateRejected = new Date();
       }

       $scope.getEndOfDayStamp = function (d) {
           var m = moment(d);
           return m.endOf('day').unix();
       }

       $scope.getStartOfDayStamp = function (d) {
           var m = moment(d);
           return m.startOf('day').unix();
       }

       // Event handlers

       $scope.pageSizeChanged = function () {
           if ($scope.activeTab == 'pending') {
               $scope.gridContainer.pendingGrid.dataSource.pageSize($scope.gridContainer.pendingGridPageSize);
           }
           else if ($scope.activeTab == 'accepted') {
               $scope.gridContainer.acceptedGrid.dataSource.pageSize($scope.gridContainer.acceptedGridPageSize);
           }
           else if ($scope.activeTab == 'rejected') {
               $scope.gridContainer.rejectedGrid.dataSource.pageSize($scope.gridContainer.rejectedGridPageSize);
           }
       }

       $scope.clearClicked = function () {

           $scope.updateActiveTab("");
       }

       $scope.tabClicked = function (tab) {
           $scope.activeTab = tab;
       }

       $scope.searchClicked = function () {
           var from, to;

           if ($scope.activeTab == 'pending') {
               from = $scope.getStartOfDayStamp($scope.dateContainer.fromDatePending);
               to = $scope.getEndOfDayStamp($scope.dateContainer.toDatePending);
           }
           else if ($scope.activeTab == 'accepted') {
               from = $scope.getStartOfDayStamp($scope.dateContainer.fromDateAccepted);
               to = $scope.getEndOfDayStamp($scope.dateContainer.toDateAccepted);
           }
           else if ($scope.activeTab == 'rejected') {
               from = $scope.getStartOfDayStamp($scope.dateContainer.fromDateRejected);
               to = $scope.getEndOfDayStamp($scope.dateContainer.toDateRejected);
           }

           var q = "DriveDateTimestamp ge " + from + " and DriveDateTimestamp le " + to;
           $scope.updateActiveTab(q);
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

           modalInstance.result.then(function (res) {
               Report.delete({ id: id }, function () {
                   var from = $scope.getStartOfDayStamp($scope.dateContainer.fromDatePending);
                   var to = $scope.getEndOfDayStamp($scope.dateContainer.toDatePending);
                   var q = "DriveDateTimestamp ge " + from + " and DriveDateTimestamp le " + to;
                   $scope.updatePendingReports(q);
               });
           });
       }

       $scope.editClick = function (id) {
           // Create a new scope to inject into EditReportController
           var scope = $rootScope.$new();

           // Get the report from the server
           Report.get({ id: id }, function (data) {
               // Set values in the scope.
               scope.purpose = data[0].purpose;
               scope.driveDate = moment.unix(data[0].driveDateTimestamp).format("DD/MM/YYYY");
           });




           console.log("Ive been clicked");

           var modalInstance = $modal.open({
               scope: scope,
               templateUrl: '/App/MyReports/EditReportModal.html',
               controller: 'EditReportController',
               windowClass: 'full',
           });

           modalInstance.result.then(function (itemId) {

           });
       }

       $scope.dateChanged = function () {

           //TODO: Shit doesnt work if the input field is left empty. It yields NaN and gives no results.

           // $timeout is a bit of a hack, but it is needed to get the current input value because ng-change is called before ng-model updates.
           $timeout(function () {
               var from, to, and;

               if ($scope.activeTab == 'pending') {
                   and = " and ";
                   from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDatePending);
                   to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDatePending);
               }
               else if ($scope.activeTab == 'accepted') {
                   and = " and ";
                   from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDateAccepted);
                   to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDateAccepted);
               }
               else if ($scope.activeTab == 'rejected') {
                   and = " and ";
                   from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDateRejected);
                   to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDateRejected);
               }


               $scope.updateActiveTab(to + and + from);
           }, 0);


       }


       // Init


       // Load up the grids.
       $scope.loadAcceptedReports();
       $scope.loadRejectedReports();
       $scope.loadPendingReports();


       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
       $scope.dateContainer = {};

       $scope.loadInitialDates();

       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",
       };

       $scope.gridContainer.pendingGridPageSize = 5;
       $scope.gridContainer.acceptedGridPageSize = 5;
       $scope.gridContainer.rejectedGridPageSize = 5;

       // Set activeTab's initial value to pending.
       $scope.activeTab = "pending";


   }
]);