angular.module("application").controller("ApproveReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", function ($scope, $modal, $rootScope, Report, OrgUnit) {



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



           $scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending' " + and + oDataQuery + "&$expand=Employment"
           $scope.gridContainer.pendingGrid.dataSource.read();


       }

       $scope.updateAcceptedReports = function (oDataQuery) {

           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.acceptedGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted' " + and + oDataQuery + "&$expand=Employment"
           $scope.gridContainer.acceptedGrid.dataSource.read();
       }

       $scope.updateRejectedReports = function (oDataQuery) {
           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.rejectedGrid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected' " + and + oDataQuery + "&$expand=Employment"
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
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending'&$expand=Employment",
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

                           var leaderOrgId = 2;
                           var resultSet = [];

                           var orgs = OrgUnit.get();
                           var orgUnits = {};

                           orgs.$promise.then(function (res) {
                               angular.forEach(orgs.value, function (value, key) {
                                   orgUnits[value.Id] = value;
                               });

                               angular.forEach(data.value, function (value, key) {
                                   var repOrg = orgUnits[value.Employment.OrgUnitId];

                                   if (orgUnits[leaderOrgId].Level == repOrg.Level && orgUnits[leaderOrgId].Id == repOrg.Id) {
                                     
                                       resultSet.push(value);
                                   }
                                   else if (orgUnits[leaderOrgId].Level < repOrg.Level) {
                                       while (orgUnits[leaderOrgId].Level < repOrg.Level) {
                                           repOrg = orgUnits[repOrg.ParentId];
                                       }
                                       if (repOrg.Id == orgUnits[leaderOrgId].Id) {
                                           resultSet.push(value);
                                       }
                                   }

                               });

                               $scope.gridContainer.pendingGrid.dataSource.data(resultSet);
                               $scope.gridContainer.pendingGrid.refresh();

                           });
                           return resultSet;

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
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted'&$expand=Employment",
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

                           var leaderOrgId = 2;
                           var resultSet = [];

                           var orgs = OrgUnit.get();
                           var orgUnits = {};

                           orgs.$promise.then(function (res) {
                               angular.forEach(orgs.value, function (value, key) {
                                   orgUnits[value.Id] = value;
                               });

                               angular.forEach(data.value, function (value, key) {
                                   var repOrg = orgUnits[value.Employment.OrgUnitId];

                                   if (orgUnits[leaderOrgId].Level == repOrg.Level && orgUnits[leaderOrgId].Id == repOrg.Id) {
                                       resultSet.push(value);
                                   }
                                   else if (orgUnits[leaderOrgId].Level < repOrg.Level) {
                                       while (orgUnits[leaderOrgId].Level < repOrg.Level) {
                                           repOrg = orgUnits[repOrg.ParentId];
                                       }
                                       if (repOrg.Id == orgUnits[leaderOrgId].Id) {
                                           resultSet.push(value);
                                       }
                                   }

                               });

                               $scope.gridContainer.acceptedGrid.dataSource.data(resultSet);
                               $scope.gridContainer.acceptedGrid.refresh();

                           });







                           return resultSet;

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
                       field: "Purpose",
                       title: "Formål"
                   }, {
                       field: "Type",
                       title: "Type"
                   }
               ]
           };
       }


       $scope.states = ['Alabama', 'Alaska', 'Arizona', 'Arkansas', 'California', 'Colorado', 'Connecticut', 'Delaware', 'Florida', 'Georgia', 'Hawaii', 'Idaho', 'Illinois', 'Indiana', 'Iowa', 'Kansas', 'Kentucky', 'Louisiana', 'Maine', 'Maryland', 'Massachusetts', 'Michigan', 'Minnesota', 'Mississippi', 'Missouri', 'Montana', 'Nebraska', 'Nevada', 'New Hampshire', 'New Jersey', 'New Mexico', 'New York', 'North Dakota', 'North Carolina', 'Ohio', 'Oklahoma', 'Oregon', 'Pennsylvania', 'Rhode Island', 'South Carolina', 'South Dakota', 'Tennessee', 'Texas', 'Utah', 'Vermont', 'Virginia', 'Washington', 'West Virginia', 'Wisconsin', 'Wyoming'];


       $scope.loadRejectedReports = function () {
           $scope.rejectedReports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected'&$expand=Employment",
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
                           // Hardcoded leaderOrgId until we can get it from AD
                           var leaderOrgId = 2;


                           var resultSet = [];

                           var orgs = OrgUnit.get();

                           var orgUnits = {};

                           orgs.$promise.then(function (res) {

                               angular.forEach(orgs.value, function (value, key) {
                                   orgUnits[value.Id] = value;
                               });

                               angular.forEach(data.value, function (value, key) {
                                   var repOrg = orgUnits[value.Employment.OrgUnitId];

                                 

                                   if (orgUnits[leaderOrgId].Level == repOrg.Level && orgUnits[leaderOrgId].Id == repOrg.Id) {
                                       resultSet.push(value);
                                   }
                                   else if (orgUnits[leaderOrgId].Level < repOrg.Level) {
                                       while (orgUnits[leaderOrgId].Level < repOrg.Level) {
                                           repOrg = orgUnits[repOrg.ParentId];
                                       }
                                       if (repOrg.Id == orgUnits[leaderOrgId].Id) {
                                           resultSet.push(value);
                                       }
                                   }

                               });

                               $scope.gridContainer.rejectedGrid.dataSource.data(resultSet);
                               $scope.gridContainer.rejectedGrid.refresh();

                           });







                           return resultSet;

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

       $scope.clearName = function () {
           $scope.chosenPerson = "";
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
               Report.patch({ id: id }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix() }, function () {
                   $scope.updatePendingReports("");
                   $scope.updateAcceptedReports("");
               });
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
               Report.patch({ id: id }, { "Status": "Rejected", "ClosedDateTimestamp": moment().unix() }, function () {
                   $scope.updatePendingReports("");
                   $scope.updateRejectedReports("");
               });
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