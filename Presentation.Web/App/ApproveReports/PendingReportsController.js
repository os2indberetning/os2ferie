angular.module("application").controller("PendingReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService) {

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};



       $scope.checkboxes = {};

       var checkedReports = [];

       var allReports = [];

       // Helper Methods

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



           $scope.gridContainer.grid.dataSource.transport.options.read.url = url;
           $scope.gridContainer.grid.dataSource.read();
       }





       $scope.reports = {
           dataSource: {
               type: "odata-v4",
               transport: {
                   read: {
                       url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending'&$expand=Employment",
                   },

               },
               schema: {
                   data: function (data) {
                       var leaderOrgId = 1;
                       var resultSet = [];
                       var orgs = OrgUnit.get();
                       orgs.$promise.then(function (res) {
                           resultSet = $scope.filterReportsByLeaderOrg(orgs, data, leaderOrgId);
                           allReports = resultSet;
                           $scope.gridContainer.grid.dataSource.data(resultSet);
                           $scope.gridContainer.grid.refresh();
                       });
                       return resultSet;

                   },
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
               $scope.getCurrentPageSums();
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
                   title: "Formål",
                   template: function (data) {
                       if (data.Comment != "") {
                           return data.Purpose + "<button kendo-tooltip k-position=\"'right'\" k-content=\"'" + data.Comment + "'\" class=\"transparent-background pull-right no-border inline\"><i class=\"fa fa-comment-o\"></i></button>";
                       }
                       return data.Purpose;

                   }
               }, {
                   field: "AmountToReimburse",
                   title: "Beløb",
                   format:"{0:n2}",
                   footerTemplate: "Side: {{currentPageAmountSum}}, total: #= kendo.toString(sum, '0.00') # "
               }, {
                   field: "Distance",
                   title: "Afstand",
                   format: "{0:n2}",
                   footerTemplate: "Side: {{currentPageDistanceSum}}, total: #= kendo.toString(sum, '0.00') # "
               }, {
                   field: "Id",
                   template: "<a ng-click=approveClick(${Id})>Godkend</a> | <a ng-click=rejectClick(${Id})>Afvis</a> | <a ng-click=ApproveWithAccountClick(${Id})>Godkend med anden kontering</a><div class='col-md-1 pull-right'><input type='checkbox' ng-model='checkboxes[${Id}]' ng-change='rowChecked(${Id})'></input></div>",
                   title: "Muligheder",
                   footerTemplate: "<div class='pull-left' kendo-toolbar k-options='approveAllToolbar'></div><div class='pull-right' kendo-toolbar k-options='approveSelectedToolbar'></div>"
               }
           ],
       };



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
           $scope.dateContainer.toDate = new Date();
           $scope.dateContainer.fromDate = new Date();
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
           $scope.gridContainer.grid.dataSource.pageSize(Number($scope.gridContainer.gridPageSize));
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
           $scope.person.chosenPerson = "";
           $scope.updateReports();

       }

       function approveAllClick() {
           if (!allReports.length > 0) {
               NotificationService.AutoFadeNotification("danger", "Ingen indberetninger", "Der findes ingen indberetninger på siden!");
               return;
           }
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/Modals/ConfirmApproveAllTemplate.html',
               controller: 'AcceptController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return -1;
                   },
                   pageNumber: function () {
                       return $scope.gridContainer.grid.dataSource.page();
                   }
               }
           });

           modalInstance.result.then(function () {
               var pageNumber = $scope.gridContainer.grid.dataSource.page();
               var pageSize = $scope.gridContainer.grid.dataSource.pageSize();
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
           var pageNumber = $scope.gridContainer.grid.dataSource.page();
           var pageSize = $scope.gridContainer.grid.dataSource.pageSize();
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
           $scope.currentPageAmountSum = resAmount.toFixed(2);
           $scope.currentPageDistanceSum = resDistance.toFixed(2);

       }

       function approveAllWithAccountClick() {
           if (!allReports.length > 0) {
               NotificationService.AutoFadeNotification("danger", "Ingen indberetninger", "Der findes ingen indberetninger på siden!");
               return;
           }
           var modalInstance = $modal.open({
               templateUrl: '/App/ApproveReports/Modals/ConfirmApproveAllWithAccountTemplate.html',
               controller: 'AcceptWithAccountController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return -1;
                   },
                   pageNumber: function () {
                       return $scope.gridContainer.grid.dataSource.page();
                   }
               }
           });

           modalInstance.result.then(function (accountNumber) {
               var pageNumber = $scope.gridContainer.grid.dataSource.page();
               var pageSize = $scope.gridContainer.grid.dataSource.pageSize();
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
               templateUrl: '/App/ApproveReports/Modals/ConfirmApproveTemplate.html',
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
                   templateUrl: '/App/ApproveReports/Modals/ConfirmApproveSelectedWithAccountTemplate.html',
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
                           debugger;
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
                   templateUrl: '/App/ApproveReports/Modals/ConfirmApproveSelectedTemplate.html',
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
               templateUrl: '/App/ApproveReports/Modals/ConfirmApproveWithAccountTemplate.html',
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
               templateUrl: '/App/ApproveReports/Modals/ConfirmRejectTemplate.html',
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


       var initialLoad = 2;
       $scope.dateChanged = function () {
           // $timeout is a bit of a hack, but it is needed to get the current input value because ng-change is called before ng-model updates.
           $timeout(function () {
               var from, to, and;
               and = " and ";
               from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
               to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDate);


               // Initial load is also a bit of a hack.
               // dateChanged is called twice when the default values for the datepickers are set.
               // This leads to sorting the grid content on load, which is not what we want.
               // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
               if (initialLoad <= 0) {

                   $scope.updateReports(to + and + from);
               }
               initialLoad--;
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

       $scope.personChanged = function (item) {
           queryOptions.personQuery = "PersonId eq " + item.Id;
           $scope.updateReports();
       }

       var queryOptions = { dateQuery: "", personQuery: "" };
       $scope.person.chosenPerson = "";


       // Set initial value for grid pagesize
       $scope.gridContainer.gridPageSize = 5;

       Person.getAll().$promise.then(function (res) {
           angular.forEach(res.value, function (value, key) {
               $scope.people.push({ Id: value.Id, FullName: value.FullName });
           });
       });





   }
]);