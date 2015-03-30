angular.module("application").controller("PendingReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService) {

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};
       $scope.orgUnit = {};
       $scope.orgUnits = [];

       //Hardcoded personid 
       var personId = 1;

       OrgUnit.get().$promise.then(function (res) {
           $scope.orgUnits = res.value;
       });

       $scope.orgUnitChanged = function (item) {
           $scope.applyOrgUnitFilter($scope.orgUnit.chosenUnit);
       }


       $scope.checkAllBox = {};

       $scope.checkboxes = {};

       var checkedReports = [];

       var allReports = [];

       // Helper Methods

       $scope.approveSelectedToolbar = {
           resizable: false,
           items: [

               {
                   type: "splitButton",
                   text: "Godkend markerede",
                   click: approveSelectedClick,
                   menuButtons: [
                       { text: "Godkend markerede med anden kontering", click: approveSelectedWithAccountClick }
                   ]
               }
           ]
       };

       $scope.applyOrgUnitFilter = function (shortDescription) {
           var oldFilters = $scope.gridContainer.grid.dataSource.filter();
           var newFilters = [];


           if (oldFilters == undefined) {
               // If no filters exist, just add the filters.
               if (shortDescription != "") {
                   newFilters.push({ field: "Employment.OrgUnit.ShortDescription", operator: "eq", value: shortDescription });
               }
           } else {
               // If filters already exist then get the old filters, that arent drivedate.
               // Then add the new drivedate filters to these.
               angular.forEach(oldFilters.filters, function (value, key) {
                   if (value.field != "Employment.OrgUnit.ShortDescription") {
                       newFilters.push(value);
                   }
               });
               if (shortDescription != "") {
                   newFilters.push({ field: "Employment.OrgUnit.ShortDescription", operator: "eq", value: shortDescription });
               }

           }
           $scope.gridContainer.grid.dataSource.filter(newFilters);
       }

       $scope.applyDateFilter = function (fromDateStamp, toDateStamp) {

           var oldFilters = $scope.gridContainer.grid.dataSource.filter();
           var newFilters = [];


           if (oldFilters == undefined) {
               // If no filters exist, just add the filters.
               newFilters.push({ field: "DriveDateTimestamp", operator: "ge", value: fromDateStamp });
               newFilters.push({ field: "DriveDateTimestamp", operator: "le", value: toDateStamp });
           } else {
               // If filters already exist then get the old filters, that arent drivedate.
               // Then add the new drivedate filters to these.
               angular.forEach(oldFilters.filters, function (value, key) {
                   if (value.field != "DriveDateTimestamp") {
                       newFilters.push(value);
                   }
               });
               newFilters.push({ field: "DriveDateTimestamp", operator: "ge", value: fromDateStamp });
               newFilters.push({ field: "DriveDateTimestamp", operator: "le", value: toDateStamp });
           }
           $scope.gridContainer.grid.dataSource.filter(newFilters);
       }

       $scope.applyPersonFilter = function (fullName) {


           var oldFilters = $scope.gridContainer.grid.dataSource.filter();
           var newFilters = [];


           if (oldFilters == undefined) {
               // If no filters exist, just add the filters.
               if (fullName != "") {
                   newFilters.push({ field: "FullName", operator: "eq", value: fullName });
               }
           } else {
               // If filters already exist then get the old filters, that arent drivedate.
               // Then add the new drivedate filters to these.
               angular.forEach(oldFilters.filters, function (value, key) {
                   if (value.field != "FullName") {
                       newFilters.push(value);
                   }
               });
               if (fullName != "") {
                   newFilters.push({ field: "FullName", operator: "eq", value: fullName });
               }
               
           }
           $scope.gridContainer.grid.dataSource.filter(newFilters);
       }

       $scope.removePersonFilter = function () {
           var oldFilters = $scope.gridContainer.grid.dataSource.filter();
           if (oldFilters == undefined) {
               return;
           }

           var newFilters = [];
           angular.forEach(oldFilters.filters, function (value, key) {
               if (value.field != "FullName") {
                   newFilters.push(value);
               }
           });
           $scope.gridContainer.grid.dataSource.filter(newFilters);
       }

       $scope.removeDateFilter = function() {
           var oldFilters = $scope.gridContainer.grid.dataSource.filter();
           if (oldFilters == undefined) {
               return;
           }
           var newFilters = [];
           angular.forEach(oldFilters.filters, function (value, key) {
               if (value.field != "DriveDateTimestamp") {
                   newFilters.push(value);
               }
           });
           $scope.gridContainer.grid.dataSource.filter(newFilters);
       }

       $scope.reports = {
           dataSource:
               {
                   type: "odata-v4",
                   transport: {
                       read: {
                           url: "/odata/DriveReports?$expand=Employment($expand=OrgUnit),DriveReportPoints",
                       },

                   },
                   schema: {
                       data: function (data) {
                           allReports = data.value;
                           return data.value;
                       },
                   },
                   pageSize: 1,
                   serverPaging: true,
                   serverAggregates: false,
                   serverSorting: true,
                   serverFiltering: true,
                   sort: [{ field: "FullName", dir: "desc" }, { field: "DriveDateTimestamp", dir: "desc" }],
                  // filter: [{ field: "Status", operator: "eq", value: "Core.DomainModel.ReportStatus%270%27" }]
               },

           sortable: { mode: "multiple" },
           resizable: true,
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
               },
               pageSizes: [5, 10, 20, 30, 40, 50]
           },
           scrollable: false,
           dataBound: function () {
               $scope.getCurrentPageSums();
               $scope.getAllPagesSums();
               this.expandRow(this.tbody.find("tr.k-master-row").first());
           },
           columns: [
               {
                   field: "FullName",
                   title: "Medarbejder"
               }, {
                   field: "Employment.OrgUnit.ShortDescription",
                   title: "Organisationsenhed"
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
                   title: "Formål",
               }, {
                   title: "Rute",
                   field: "DriveReportPoints",
                   template: function (data) {
                       var tooltipContent = "";
                       var gridContent = "";
                       angular.forEach(data.DriveReportPoints, function (point, key) {
                           if (key != data.DriveReportPoints.length - 1) {
                               tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town + "<br/>";
                               gridContent += point.Town + "<br/>";
                           } else {
                               tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town;
                               gridContent += point.Town;
                           }
                       });
                       var result = "<div kendo-tooltip k-content=\"'" + tooltipContent + "'\">" + gridContent + "</div>";

                       if (data.KilometerAllowance != "Read") {
                           return result;
                       } else {
                           if (data.IsFromApp) {
                               return "<div kendo-tooltip k-content=\"'" + data.UserComment + "'\">Aflæst fra GPS</div>";
                           } else {
                               return "<div kendo-tooltip k-content=\"'" + data.UserComment + "'\">Aflæst manuelt</div>";
                           }

                       }
                   }
               }, {
                   field: "Distance",
                   title: "Km",
                   template: function (data) {
                       return data.Distance.toFixed(2).toString().replace('.', ',') + " Km";
                   },
                   footerTemplate: "Side: {{currentPageDistanceSum}} KM <br/> Total: {{allPagesDistanceSum}} KM"
               }, {
                   field: "AmountToReimburse",
                   title: "Beløb",
                   template: function (data) {
                       return data.AmountToReimburse.toFixed(2).toString().replace('.', ',') + " Dkk.";
                   },
                   footerTemplate: "Side: {{currentPageAmountSum}} Dkk. <br/> Total: {{allPagesAmountSum}} Dkk."
               }, {
                   field: "KilometerAllowance",
                   title: "Merkørsel",
                   template: function (data) {
                       if (data.KilometerAllowance == "CalculatedWithoutExtraDistance") {
                           return "<i class='fa fa-check'></i>";
                       }
                       return "";
                   }
               }, {
                   field: "FourKmRule",
                   title: "4 km",
                   template: function (data) {
                       if (data.FourKmRule) {
                           return "<i class='fa fa-check'></i>";
                       }
                       return "";
                   }
               }, {
                   field: "CreatedDateTimestamp",
                   title: "Indberetningsdato",
                   template: function (data) {
                       var m = moment.unix(data.CreatedDateTimestamp);
                       return m._d.getDate() + "/" +
                           (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                           m._d.getFullYear();
                   },
               }, {
                   sortable: false,
                   field: "Id",
                   template: "<a ng-click=approveClick(${Id})>Godkend</a> | <a ng-click=rejectClick(${Id})>Afvis</a> <div class='col-md-1 pull-right'><input type='checkbox' ng-model='checkboxes[${Id}]' ng-change='rowChecked(${Id})'></input></div>",
                   headerTemplate: "Muligheder <div class='col-md-1 pull-right'><input ng-change='checkAllBoxesOnPage()' type='checkbox' ng-model='checkAllBox.isChecked'></input></div>",
                   footerTemplate: "<div class='pull-right fill-width' kendo-toolbar k-options='approveSelectedToolbar'></div>"
               }
           ],
       };

       $scope.checkAllBoxesOnPage = function () {
           if ($scope.checkAllBox.isChecked) {
               var pageNumber = $scope.gridContainer.grid.dataSource.page();
               var pageSize = $scope.gridContainer.grid.dataSource.pageSize();
               var first = pageSize * (pageNumber - 1);
               var last = first + pageSize - 1;
               for (var i = first; i <= last; i++) {
                   if (allReports[i] != undefined) {
                       var repId = allReports[i].Id;
                       $scope.checkboxes[repId] = true;
                       checkedReports.push(repId);
                   }

               }
           } else {
               var pageNumber = $scope.gridContainer.grid.dataSource.page();
               var pageSize = $scope.gridContainer.grid.dataSource.pageSize();
               var first = pageSize * (pageNumber - 1);
               var last = first + pageSize - 1;
               for (var i = first; i <= last; i++) {
                   if (allReports[i] != undefined) {
                       var repId = allReports[i].Id;
                       $scope.checkboxes[repId] = false;
                       var index = checkedReports.indexOf(repId);
                       checkedReports.splice(index, 1);
                   }
               }
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



       $scope.loadInitialDates = function () {
           // Set initial values for kendo datepickers.

           initialLoad = 2;

           var from = new Date();
           from.setDate(from.getDate() - 30);

           $scope.dateContainer.toDate = new Date();
           $scope.dateContainer.fromDate = from;


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
           $scope.loadInitialDates();
           $scope.removeDateFilter();
           $scope.removePersonFilter();
           $scope.person.chosenPerson = "";
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
           $scope.currentPageAmountSum = resAmount.toFixed(2).toString().replace('.', ',');
           $scope.currentPageDistanceSum = resDistance.toFixed(2).toString().replace('.', ',');

       }

       $scope.getAllPagesSums = function () {
           var resAmount = 0;
           var resDistance = 0;
           angular.forEach(allReports, function (rep, key) {
               resAmount += rep.AmountToReimburse;
               resDistance += rep.Distance;
           });
           $scope.allPagesAmountSum = resAmount.toFixed(2).toString().replace('.', ',');
           $scope.allPagesDistanceSum = resDistance.toFixed(2).toString().replace('.', ',');
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
               var from = $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
               var to = $scope.getEndOfDayStamp($scope.dateContainer.toDate);

               // Initial load is also a bit of a hack.
               // dateChanged is called twice when the default values for the datepickers are set.
               // This leads to sorting the grid content on load, which is not what we want.
               // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
               if (initialLoad <= 0) {
                   $scope.applyDateFilter(from, to);
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
           $scope.applyPersonFilter($scope.person.chosenPerson);

       }

       $scope.person.chosenPerson = "";

       Person.getAll().$promise.then(function (res) {
           angular.forEach(res.value, function (value, key) {
               $scope.people.push({ Id: value.Id, FullName: value.FullName });
           });
       });





   }
]);