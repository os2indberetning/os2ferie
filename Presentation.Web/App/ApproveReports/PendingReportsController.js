angular.module("application").controller("PendingReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService) {


       var queryOptions = { dateQuery: "", personQuery: "" };

       $scope.checkboxes = {};

       var checkedReports = [];

       var allReports = [];

       // Helper Methods

       $scope.test = 2;


       $scope.approveAllToolbar = {
           items: [

               {
                   type: "splitButton",
                   text: "Godkend Alle",
                   click: approveAllClick,
                   menuButtons: [
                       { text: "Anden Kontering", click: approveAllWithAccountClick }
                   ]
               },
           ]
       };

       $scope.approveSelectedToolbar = {
           items: [

               {
                   type: "splitButton",
                   text: "Godkend Markerede",
                   click: approveSelectedClick,
                   menuButtons: [
                       { text: "Anden Kontering", click: approveSelectedWithAccountClick }
                   ]
               }
           ]
       };





       $scope.updateReports = function () {

           var dateAnd = "and ";
           var dateQuery = dateAnd + queryOptions.dateQuery;
           if (queryOptions.dateQuery == "") {
               dateAnd = "";
               dateQuery = "";
           }

           var personAnd = "and ";
           var personQuery = personAnd + queryOptions.personQuery;
           if (queryOptions.personQuery == "") {
               personAnd = "";
               personQuery = "";
           }

           var query = dateQuery + personQuery;

           var url = "/odata/DriveReports?$expand=Employment &$filter=Status eq Core.DomainModel.ReportStatus'Pending' " + query;



           $scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = url;
           $scope.gridContainer.pendingGrid.dataSource.read();
       }





       $scope.loadReports = function () {
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

                           var leaderOrgId = 1;
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
                       refresh: "Genopfrisk",
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
                       footerTemplate: "Side: " + $scope.currentPageAmountSum + " Total: #= sum # "
                   }, {
                       field: "Distance",
                       title: "Afstand",
                       footerTemplate: "Total: #= sum # "
                   }, {
                       field: "Id",
                       template: "<a ng-click=approveClick(${Id})>Godkend</a> | <a ng-click=rejectClick(${Id})>Afvis</a> | <a ng-click=ApproveWithAccountClick(${Id})>Godkend med anden kontering</a><div class='col-md-1 pull-right'><input type='checkbox' ng-model='checkboxes[${Id}]' ng-change='rowChecked(${Id})'/></div>",
                       title: "Muligheder",
                       footerTemplate: "<div class='pull-left' kendo-toolbar k-options='approveAllToolbar'></div><div class='pull-right' kendo-toolbar k-options='approveSelectedToolbar'></div>"
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





       $scope.loadInitialDates = function () {
           // Set initial values for kendo datepickers.
           $scope.dateContainer.toDatePending = new Date();
           $scope.dateContainer.fromDatePending = new Date();
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
           $scope.gridContainer.pendingGrid.dataSource.pageSize(Number($scope.gridContainer.pendingGridPageSize));
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
           queryOptions.dateQuery = "";
           queryOptions.personQuery = "";
           $scope.person.pendingChosenPerson = "";
           $scope.updateReports();

       }

       function approveAllClick() {
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/ConfirmApproveAllTemplate.html',
               controller: 'AcceptController',
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
                           $scope.updateReports();
                       });

                   }

               }



               if (allReports.length > noOfApprovedReports) {
                   NotificationService.AutoFadeNotification("warning", "Yderligere indberetninger", "Der findes flere sider til godkendelse!");
               }
           });


       }

       $scope.getCurrentPageSums = function () {
           var pageNumber = $scope.gridContainer.pendingGrid.dataSource.page();
           var pageSize = $scope.gridContainer.pendingGrid.dataSource.pageSize();
           var first = pageSize * (pageNumber - 1);
           var last = first + pageSize - 1;
           var resAmount = 0;
           var resDistance = 0;
           for (var i = first; i <= last; i++) {
               if (allReports[i] != undefined) {
                   resAmount += allReports[i].AmountToReimburse;
                   resDistance += allReports[i].Distance;
               }

           }
           $scope.currentPageAmountSum = resAmount;
           $scope.currentPageDistanceSum = resDistance;

       }

       function approveAllWithAccountClick() {
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
                       Report.patch({ id: allReports[i].Id }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix(), "AccountNumber": accountNumber }, function () {
                           $scope.updateReports();
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
                   $scope.updateReports();
               });
           });
       }

       function approveSelectedWithAccountClick() {
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
                       Report.patch({ id: value }, { "Status": "Accepted", "ClosedDateTimestamp": moment().unix(), "AccountNumber": accountNumber }, function () {
                           $scope.updateReports();
                       });
                   });
                   checkedReports = [];
               });
           }
       }

       function approveSelectedClick() {
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
                           $scope.updateReports();

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
                   $scope.updateReports();

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
                   $scope.updateReports();

               });
           });
       }



       $scope.dateChanged = function () {

           //TODO: Shit doesnt work if the input field is left empty. It yields NaN and gives no results.

           // $timeout is a bit of a hack, but it is needed to get the current input value because ng-change is called before ng-model updates.
           $timeout(function () {
               var from, to, and;
               and = " and ";
               from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDatePending);
               to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDatePending);
               queryOptions.dateQuery = from + and + to;
               $scope.updateReports();
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




       $scope.loadReports();



       $scope.personChanged = function (item) {
           queryOptions.personQuery = "PersonId eq " + item.Id;
           $scope.updateReports();
       }

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};

       // Set initial value for grid pagesize
       $scope.gridContainer.pendingGridPageSize = 5;

       Person.getAll().$promise.then(function (res) {
           angular.forEach(res.value, function (value, key) {
               $scope.people.push({ Id: value.Id, FullName: value.FullName });
           });
       });

       $scope.$on("kendoWidgetCreated", function (event, widget) {
           if (widget === $scope.gridContainer.pendingGrid) {
               $scope.getCurrentPageSums();
           }
       });


   }
]);