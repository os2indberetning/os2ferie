angular.module("application").controller("ApproveReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService) {


       var pendingQueryOptions = { dateQuery: "", personQuery: "" };
       var acceptedQueryOptions = { dateQuery: "", personQuery: "" };
       var rejectedQueryOptions = { dateQuery: "", personQuery: "" };


       $scope.checkboxes = {};

       var checkedReports = [];

       var allReports = [];

       // Helper Methods

       $scope.updateActiveTab = function () {
           if ($scope.activeTab == 'pending') {
               // Update pending tabs.
               this.updatePendingReports();
           }
           else if ($scope.activeTab == 'accepted') {
               // Update accepted reports grid
               this.updateAcceptedReports();
           }
           else if ($scope.activeTab == 'rejected') {
               // Update rejected reports grid.
               this.updateRejectedReports();
           }
       }

       $scope.updatePendingReports = function () {

           var dateAnd = "and ";
           var dateQuery = dateAnd + pendingQueryOptions.dateQuery;
           if (pendingQueryOptions.dateQuery == "") {
               dateAnd = "";
               dateQuery = "";
           }

           var personAnd = "and ";
           var personQuery = personAnd + pendingQueryOptions.personQuery;
           if (pendingQueryOptions.personQuery == "") {
               personAnd = "";
               personQuery = "";
           }

           var query = dateQuery + personQuery;

           var url = "/odata/DriveReports?$expand=Employment &$filter=Status eq Core.DomainModel.ReportStatus'Pending' " + query;



           $scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = url;
           $scope.gridContainer.pendingGrid.dataSource.read();
       }

       $scope.updateAcceptedReports = function () {
           var dateAnd = "and ";
           var dateQuery = dateAnd + acceptedQueryOptions.dateQuery;
           if (acceptedQueryOptions.dateQuery == "") {
               dateAnd = "";
               dateQuery = "";
           }

           var personAnd = "and ";
           var personQuery = personAnd + acceptedQueryOptions.personQuery;
           if (acceptedQueryOptions.personQuery == "") {
               personAnd = "";
               personQuery = "";
           }

           var query = dateQuery + personQuery;

           var url = "/odata/DriveReports?$expand=Employment &$filter=Status eq Core.DomainModel.ReportStatus'Accepted' " + query;

           $scope.gridContainer.acceptedGrid.dataSource.transport.options.read.url = url;
           $scope.gridContainer.acceptedGrid.dataSource.read();
       }

       $scope.updateRejectedReports = function () {
           var dateAnd = "and ";
           if (rejectedQueryOptions.dateQuery == "") {
               dateAnd = "";
           }
           var dateQuery = dateAnd + rejectedQueryOptions.dateQuery;

           var personAnd = "and ";
           if (rejectedQueryOptions.personQuery == "") {
               personAnd = "";
           }
           var personQuery = personAnd + rejectedQueryOptions.personQuery;

           var query = dateQuery + personQuery;

           var url = "/odata/DriveReports?$expand=Employment &$filter=Status eq Core.DomainModel.ReportStatus'Rejected' " + query;



           $scope.gridContainer.rejectedGrid.dataSource.transport.options.read.url = url;
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
                       model: {
                           fields: {
                               Distance: { type: "number" },
                               AmountToReimburse: { type: "number" }
                           }
                       },
                       data: function (data) {

                           var leaderOrgId = 2;
                           var resultSet = [];

                           var orgs = OrgUnit.get();

                         

                           orgs.$promise.then(function (res) {

                               resultSet = $scope.filterReportsByLeaderOrg(orgs, data, leaderOrgId);
                               allReports = resultSet;
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
                   serverAggregates: false,
                   serverSorting: true,


                   aggregate: [
                       { field: "Distance", aggregate: "sum" },
                       { field: "AmountToReimburse", aggregate: "sum" },

                   ]
               },
               sortable: true,
               pageable: {
                   messages: {
                       display: "{0} - {1} af {2} indberetninger", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                       empty: "Ingen indberetninger at vise",
                       page: "Side",
                       of: "af {0}", //{0} is total amount of pages
                       itemsPerPage: "indberetninger pr. side",
                       first: "Gå til første side",
                       previous: "Gå til forrige side",
                       next: "Gå til næste side",
                       last: "Gå til sidste side",
                       refresh: "Genopfrisk"
                   }
               },
               scrollable: false,
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
                       field: "AmountToReimburse",
                       title: "Beløb",
                       footerTemplate: "Total: #= sum # "
                   }, {
                       field: "Distance",
                       title: "Afstand",
                       footerTemplate: "Total: #= sum # "
                   }, {
                       field: "Id",
                       template: "<a ng-click=approveClick(${Id})>Godkend</a> | <a ng-click=rejectClick(${Id})>Afvis</a> | <a ng-click=ApproveWithAccountClick(${Id})>Godkend med anden kontering</a><div class='col-md-1 pull-right'><input type='checkbox' ng-model='checkboxes[${Id}]' ng-change='rowChecked(${Id})'/></div>",
                       title: "Muligheder",
                       footerTemplate: "<div class='btn-group dropup margin-top-30'><button type='button' ng-click='approveAllClick()' class='btn btn-success'>Godkend Alle</button><button type='button' class='btn btn-success dropdown-toggle' data-toggle='dropdown'><span class='caret'></span></button><ul class='dropdown-menu' role='menu'><li><a ng-click='approveAllWithAccountClick()'>Anden kontering</a></li></ul></div>    <div class='btn-group dropup margin-top-30 pull-right'><button type='button' ng-click='approveSelectedClick()' class='btn btn-success'><i class='fa fa-check-square'></i> Godkend Markerede</button><button type='button' class='btn btn-success dropdown-toggle' data-toggle='dropdown'><span class='caret'></span></button><ul class='dropdown-menu' role='menu'><li><a ng-click='approveSelectedWithAccountClick()'> Anden kontering</a></li></ul></div>"
                   }
               ],
           };
       }

       $scope.filterReportsByLeaderOrg = function (orgs, data, leaderOrgId) {
           var orgUnits = {};

           var resultSet = [];

           angular.forEach(orgs.value, function (value, key) {
               orgUnits[value.Id] = value;
           });

           angular.forEach(data.value, function (value, key) {
               var repOrg = orgUnits[value.Employment.OrgUnitId];

               if (orgUnits[leaderOrgId].Level == repOrg.Level && orgUnits[leaderOrgId].Id == repOrg.Id) {

                   resultSet.push(value);
               } else if (orgUnits[leaderOrgId].Level < repOrg.Level) {
                   while (orgUnits[leaderOrgId].Level < repOrg.Level) {
                       repOrg = orgUnits[repOrg.ParentId];
                   }
                   if (repOrg.Id == orgUnits[leaderOrgId].Id) {
                       resultSet.push(value);
                   }
               }

           });
           return resultSet;
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
                       model: {
                           fields: {
                               Distance: { type: "number" },
                               AmountToReimburse: { type: "number" }
                           }
                       },

                       data: function (data) {

                           var leaderOrgId = 2;
                           var resultSet = [];

                           var orgs = OrgUnit.get();

                           orgs.$promise.then(function (res) {

                               resultSet = $scope.filterReportsByLeaderOrg(orgs, data, leaderOrgId);

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
                   serverAggregates: false,
                   serverSorting: true,


                   aggregate: [{ field: "Distance", aggregate: "sum" },
                                 { field: "AmountToReimburse", aggregate: "sum" }]
               },
               sortable: true,
               pageable: {
                   messages: {
                       display: "{0} - {1} af {2} indberetninger", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                       empty: "Ingen indberetninger at vise",
                       page: "Side",
                       of: "af {0}", //{0} is total amount of pages
                       itemsPerPage: "indberetninger pr. side",
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
                       field: "AmountToReimburse",
                       title: "Beløb",
                       footerTemplate: "Total: #= sum # "
                   }, {
                       field: "Distance",
                       title: "Afstand",
                       footerTemplate: "Total: #= sum # "
                   }, {
                       field: "AccountNumber",
                       title: "Anden Kontering",
                       template: function(data) {
                           if (data.AccountNumber == "" || data.AccountNumber == null) {
                               return "Nej";
                           } else {
                               return "Ja" +  "<button kendo-tooltip k-position=\"'right'\" k-content=\"'" + data.AccountNumber + "'\" class=\"k-group btn btn-default pull-right no-border\"><i class=\"fa fa-comment-o\"></i></button>";
                           }
                       }
                   }
               ],
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

                           var leaderOrgId = 2;
                           var resultSet = [];

                           var orgs = OrgUnit.get();

                           orgs.$promise.then(function (res) {


                               resultSet = $scope.filterReportsByLeaderOrg(orgs, data, leaderOrgId);

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
               pageable: {
                   messages: {
                       display: "{0} - {1} af {2} indberetninger", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                       empty: "Ingen indberetninger at vise",
                       page: "Side",
                       of: "af {0}", //{0} is total amount of pages
                       itemsPerPage: "indberetninger pr. side",
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
                                      field: "AmountToReimburse",
                                      title: "Beløb"
                                  }, {
                                      field: "Distance",
                                      title: "Afstand"
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
           $scope.dateContainer.toDateSubstitute = new Date();
           $scope.dateContainer.fromDateSubstitute = new Date();
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
               $scope.gridContainer.pendingGrid.dataSource.pageSize(Number($scope.gridContainer.pendingGridPageSize));
           }
           else if ($scope.activeTab == 'accepted') {
               $scope.gridContainer.acceptedGrid.dataSource.pageSize(Number($scope.gridContainer.acceptedGridPageSize));
           }
           else if ($scope.activeTab == 'rejected') {
               $scope.gridContainer.rejectedGrid.dataSource.pageSize(Number($scope.gridContainer.rejectedGridPageSize));
           }
       }

       $scope.rowChecked = function (id) {
           if ($scope.checkboxes[id]) {
               // Is run if the checkbox has been checked.
               checkedReports.push(id);
           } else {
               // Is run of the checkbox has been unchecked
               var index = checkedReports.indexOf(id);
               checkedReports.splice(index, 1);
           }
       }

       $scope.clearName = function () {
           $scope.chosenPerson = "";
       }

       $scope.clearClicked = function () {
           if ($scope.activeTab == 'pending') {
               pendingQueryOptions.dateQuery = "";
               pendingQueryOptions.personQuery = "";
               $scope.person.pendingChosenPerson = "";
           }
           else if ($scope.activeTab == 'accepted') {
               acceptedQueryOptions.dateQuery = "";
               acceptedQueryOptions.personQuery = "";
               $scope.person.acceptedChosenPerson = "";

           }
           else if ($scope.activeTab == 'rejected') {
               rejectedQueryOptions.dateQuery = "";
               rejectedQueryOptions.personQuery = "";
               $scope.person.rejectedChosenPerson = "";
           }
           $scope.updateActiveTab();
       }

       $scope.tabClicked = function (tab) {
           $scope.activeTab = tab;
       }

       $scope.approveAllClick = function () {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmApproveAllTemplate.html',
               controller: 'AcceptController',
               backdrop: "static",
               resolve: {
                   itemId: function() {
                       return -1;
                   },
                   pageNumber: function() {
                       return $scope.gridContainer.pendingGrid.dataSource.page();
                   }
               }
           });

           modalInstance.result.then(function () {
               var pageNumber = $scope.gridContainer.pendingGrid.dataSource.page();
               var pageSize = $scope.gridContainer.pendingGrid.dataSource.pageSize();
               var first = pageSize * (pageNumber - 1);
               var last = first + pageSize - 1;
               var noOfApprovedReports = 0;
               for (var i = first; i <= last; i++) {
                   if (allReports[i] != undefined) {
                       noOfApprovedReports++;
                       Report.patch({ id: allReports[i].Id }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix() }, function () {
                           $scope.updatePendingReports();
                           $scope.updateAcceptedReports();
                       });
                   
                   }

               }
              
           

               if (allReports.length > noOfApprovedReports) {
                   NotificationService.AutoFadeNotification("warning", "Yderligere indberetninger", "Der findes flere sider til godkendelse!");
               }
           });
           

       }

       $scope.approveAllWithAccountClick = function() {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmApproveAllWithAccountTemplate.html',
               controller: 'AcceptWithAccountController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return -1;
                   },
                   pageNumber: function () {
                       return $scope.gridContainer.pendingGrid.dataSource.page();
                   }
               }
           });

           modalInstance.result.then(function (accountNumber) {
               var pageNumber = $scope.gridContainer.pendingGrid.dataSource.page();
               var pageSize = $scope.gridContainer.pendingGrid.dataSource.pageSize();
               var first = pageSize * (pageNumber - 1);
               var last = first + pageSize - 1;
               var noOfApprovedReports = 0;
               for (var i = first; i <= last; i++) {
                   if (allReports[i] != undefined) {
                       noOfApprovedReports++;
                       Report.patch({ id: allReports[i].Id }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix(), "AccountNumber" : accountNumber}, function () {
                           $scope.updatePendingReports();
                           $scope.updateAcceptedReports();
                       });

                   }

               }

               if (allReports.length > noOfApprovedReports) {
                   NotificationService.AutoFadeNotification("warning", "Yderligere indberetninger", "Der findes flere sider til godkendelse!");
               }
           });
       }

       $scope.approveClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmApproveTemplate.html',
               controller: 'AcceptController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return id;
                   },
                   pageNumber: -1
               }
           });

           modalInstance.result.then(function () {
               Report.patch({ id: id }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix() }, function () {
                   $scope.updatePendingReports();
                   $scope.updateAcceptedReports();
               });
           });
       }

       $scope.approveSelectedWithAccountClick = function() {
           if (checkedReports.length == 0) {
               NotificationService.AutoFadeNotification("danger", "Fejl", "Ingen indberetninger er markerede!");
           } else {
               var modalInstance = $modal.open({
                   templateUrl: '/App/ApproveReports/ConfirmApproveSelectedWithAccountTemplate.html',
                   controller: 'AcceptWithAccountController',
                   backdrop: "static",
                   resolve: {
                       itemId: function () {
                           return -1;
                       },
                       pageNumber: -1
                   }
               });

               modalInstance.result.then(function (accountNumber) {
                   angular.forEach(checkedReports, function (value, key) {
                       Report.patch({ id: value }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix(), "AccountNumber" : accountNumber}, function () {
                           $scope.updatePendingReports();
                           $scope.updateAcceptedReports();
                       });
                   });
                   checkedReports = [];
               });
           }
       }

       $scope.approveSelectedClick = function () {
           if (checkedReports.length == 0) {
               NotificationService.AutoFadeNotification("danger", "Fejl", "Ingen indberetninger er markerede!");
           } else {
               var modalInstance = $modal.open({
                   templateUrl: '/App/ApproveReports/ConfirmApproveSelectedTemplate.html',
                   controller: 'AcceptController',
                   backdrop: "static",
                   resolve: {
                       itemId: function () {
                           return -1;
                       },
                       pageNumber: -1
                   }
               });

               modalInstance.result.then(function () {
                   angular.forEach(checkedReports, function (value, key) {
                       Report.patch({ id: value }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix() }, function () {
                           $scope.updatePendingReports();
                           $scope.updateAcceptedReports();
                       });
                   });
                   checkedReports = [];
               });
           }
       }


       $scope.ApproveWithAccountClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmApproveWithAccountTemplate.html',
               controller: "AcceptWithAccountController",
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return id;
                   },
                   pageNumber: -1
               }
           });

           modalInstance.result.then(function (res) {
               Report.patch({ id: id }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix(), "AccountNumber": res.AccountNumber }, function () {
                   $scope.updatePendingReports();
                   $scope.updateAcceptedReports();
               });
           });
       }

       $scope.rejectClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmRejectTemplate.html',
               controller: 'RejectController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function (res) {
               Report.patch({ id: id }, { "Status": "Rejected", "ClosedDateTimestamp": moment().unix(), "Comment": res.Comment }, function () {
                   $scope.updatePendingReports();
                   $scope.updateRejectedReports();
               });
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
                   pendingQueryOptions.dateQuery = from + and + to;
               }
               else if ($scope.activeTab == 'accepted') {
                   and = " and ";
                   from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDateAccepted);
                   to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDateAccepted);
                   acceptedQueryOptions.dateQuery = from + and + to;
               }
               else if ($scope.activeTab == 'rejected') {
                   and = " and ";
                   from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDateRejected);
                   to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDateRejected);
                   rejectedQueryOptions.dateQuery = from + and + to;
               }

               $scope.updateActiveTab();
           }, 0);


       }

        $scope.substituteInitials = "foo";

       $scope.createNewSubstitute = function () {
           var from = $scope.dateContainer.fromDateSubstitute;
           var to = $scope.dateContainer.toDateSubstitute;
           if (from - to < 0) {
               //TODO alert user
           }
           console.log($scope.substituteInitials);
       };





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

       $scope.personChanged = function (item) {
           if ($scope.activeTab == 'pending') {
               pendingQueryOptions.personQuery = "PersonId eq " + item.Id;
           }
           else if ($scope.activeTab == 'accepted') {
               acceptedQueryOptions.personQuery = "PersonId eq " + item.Id;
           }
           else if ($scope.activeTab == 'rejected') {
               rejectedQueryOptions.personQuery = "PersonId eq " + item.Id;
           }

           $scope.updateActiveTab();
       }

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};

       // Set initial value for grid pagesizes
       $scope.gridContainer.pendingGridPageSize = 5;
       $scope.gridContainer.acceptedGridPageSize = 5;
       $scope.gridContainer.rejectedGridPageSize = 5;

       Person.getAll().$promise.then(function (res) {
           angular.forEach(res.value, function (value, key) {
               $scope.people.push({ Id: value.Id, FullName: value.FullName });
           });
       });




   }
]);