angular.module("application").controller("ApproveReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout) {


       var pendingQueryOptions = { dateQuery: "", personQuery: "" };
       var acceptedQueryOptions = { dateQuery: "", personQuery: "" };
       var rejectedQueryOptions = { dateQuery: "", personQuery: "" };

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

           console.log(url);

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

           console.log(url);

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

           console.log(url);

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
                   sortAble: true,
                   pageable: true,




                   serverPaging: false,
                   serverAggregates: false,
                   serverSorting: true,


                   aggregate: [{ field: "Distance", aggregate: "sum" },
                                 { field: "AmountToReimburse", aggregate: "sum" }]
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
                       field: "AmountToReimburse",
                       title: "Beløb",
                       footerTemplate: "Total: #= sum # "
                   }, {
                       field: "Distance",
                       title: "Afstand",
                       footerTemplate: "Total: #= sum # "
                   }, {
                       field: "Id",
                       template: "<a ng-click=approveClick(${Id})>Godkend</a> | <a ng-click=rejectClick(${Id})>Afvis</a> | <a ng-click=approveWithAccount(${Id})>Godkend med anden kontering</a>",
                       title: "Muligheder"
                   }
               ],
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
                   serverSorting: true,

                   aggregate: [{ field: "Distance", aggregate: "sum" },
                               { field: "AmountToReimburse", aggregate: "sum"}
                   ]

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
                                      field: "AmountToReimburse",
                                      title: "Beløb",
                                      footerTemplate: "Total: #= sum #"
                                  }, {
                                      field: "Distance",
                                      title: "Afstand",
                                      footerTemplate: "Total: #= sum #"
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

       $scope.approveClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmApproveTemplate.html',
               controller: 'AcceptController',
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function () {
               Report.patch({ id: id }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix() }, function () {
                   $scope.updatePendingReports();
                   $scope.updateAcceptedReports();
               });
           });
       }

       $scope.rejectClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmRejectTemplate.html',
               controller: 'RejectController',
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


       $scope.$watch('dateContainer.fromDatePending', $scope.dateChanged);

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

       Person.get().$promise.then(function (res) {
           angular.forEach(res.value, function (value, key) {
               $scope.people.push({ Id: value.Id, FullName: value.FirstName + " " + value.MiddleName + " " + value.LastName });
           });
       });


   }
]);