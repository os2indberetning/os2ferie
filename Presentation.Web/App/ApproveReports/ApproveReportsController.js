angular.module("application").controller("ApproveReportsController", [
   "$scope", "$modal", "$rootScope", "Report", function ($scope, $modal, $rootScope, Report) {



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

           $scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending' " + and + oDataQuery;
           $scope.gridContainer.pendingGrid.dataSource.read();

         
       }

       $scope.updateAcceptedReports = function (oDataQuery) {

           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.acceptedGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted' " + and + oDataQuery;
           $scope.gridContainer.acceptedGrid.dataSource.read();
       }

       $scope.updateRejectedReports = function (oDataQuery) {
           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.rejectedGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected' " + and + oDataQuery;
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
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending'",
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
                   }, {
                       field: "Id",
                       template: "<a ng-click=approveClick(${Id})>Godkend</a> | <a ng-click=rejectClick(${Id})>Afvis</a>",
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
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted'",
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

       $scope.loadRejectedReports = function () {
           $scope.rejectedReports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected'",
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

       $scope.approveClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmApproveTemplate.html',
               controller: 'AcceptRejectController',
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function () {
               Report.patch({ id: id, Status: "Accepted" });
           });
       }

       $scope.rejectClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmRejectTemplate.html',
               controller: 'AcceptRejectController',
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function () {
               Report.patch({ id: id, Status: "Rejected" });
           });
       }











       // Init

       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
       $scope.dateContainer = {};

       $scope.loadInitialDates();

       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",
       };

       // Set activeTab's initial value to pending.
       $scope.activeTab = "pending";

       // Load up the grids.
       $scope.loadAcceptedReports();
       $scope.loadRejectedReports();
       $scope.loadPendingReports();

   }
]);