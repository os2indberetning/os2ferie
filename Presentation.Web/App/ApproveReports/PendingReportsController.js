angular.module("application").controller("PendingReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", "RateType", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService, RateType) {

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};
       $scope.orgUnit = {};
       $scope.orgUnits = [];

       $scope.tableSortHelp = $rootScope.HelpTexts.TableSortHelp.text;

       // Set personId. The value on $rootScope is set in resolve in application.js
       var personId = $rootScope.CurrentUser.Id;
       $scope.isLeader = $rootScope.CurrentUser.IsLeader;

       $scope.orgUnits = $rootScope.OrgUnits;
       $scope.people = $rootScope.People;

       $scope.orgUnitAutoCompleteOptions = {
           filter: "contains",
           select: function (e) {
               $scope.orgUnit.chosenId = this.dataItem(e.item.index()).Id;
           }
       }

       $scope.personAutoCompleteOptions = {
           filter: "contains",
           select: function (e) {
               $scope.person.chosenId = this.dataItem(e.item.index()).Id;
           }
       };

       $scope.getEndOfDayStamp = function (d) {
           var m = moment(d);
           return m.endOf('day').unix();
       }

       $scope.getStartOfDayStamp = function (d) {
           var m = moment(d);
           return m.startOf('day').unix();
       }

       // dates for kendo filter.
       var fromDateFilter = new Date();
       fromDateFilter.setDate(fromDateFilter.getDate() - 365);
       fromDateFilter = $scope.getStartOfDayStamp(fromDateFilter);
       var toDateFilter = $scope.getEndOfDayStamp(new Date());

       $scope.checkAllBox = {};

       $scope.checkboxes = {};
       $scope.checkboxes.showSubbed = false;

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

       $scope.clearClicked = function () {
           /// <summary>
           /// Clears filters.
           /// </summary>
           $scope.loadInitialDates();
           $scope.person.chosenPerson = "";
           $scope.orgUnit.chosenUnit = "";
           $scope.searchClicked();
       }

       $scope.searchClicked = function () {
           var from = $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
           var to = $scope.getEndOfDayStamp($scope.dateContainer.toDate);
           $scope.gridContainer.grid.dataSource.transport.options.read.url = getDataUrl(from, to, $scope.person.chosenPerson, $scope.orgUnit.chosenUnit);
           $scope.gridContainer.grid.dataSource.read();
       }

       var getDataUrl = function (from, to, fullName, longDescription) {
           var url = "/odata/DriveReports?leaderId=" + personId + "&status=Pending" + "&getReportsWhereSubExists=" + $scope.checkboxes.showSubbed + " &$expand=Employment($expand=OrgUnit),DriveReportPoints,ResponsibleLeader";
           var filters = "&$filter=DriveDateTimestamp ge " + from + " and DriveDateTimestamp le " + to;
           if (fullName != undefined && fullName != "") {
               filters += " and PersonId eq " + $scope.person.chosenId;
           }
           if (longDescription != undefined && longDescription != "") {
               filters += " and Employment/OrgUnitId eq " + $scope.orgUnit.chosenId;
           }
           var result = url + filters;
           return result;
       }

       $scope.showSubsChanged = function () {
           /// <summary>
           /// Applies filter according to getReportsWhereSubExists
           /// </summary>
           $scope.searchClicked();
       }

       $scope.reports = {
           dataSource:
           {
               type: "odata-v4",
               transport: {
                   read: {
                       url: "/odata/DriveReports?leaderId=" + personId + "&status=Pending" + "&getReportsWhereSubExists=" + $scope.checkboxes.showSubbed + " &$expand=Employment($expand=OrgUnit),DriveReportPoints,ResponsibleLeader &$filter=DriveDateTimestamp ge " + fromDateFilter + " and DriveDateTimestamp le " + toDateFilter,
                   },

               },
               schema: {
                   data: function (data) {
                       allReports = data.value;
                       return data.value;
                   },
               },
               pageSize: 20,
               serverPaging: true,
               serverAggregates: false,
               serverSorting: true,
               serverFiltering: true,
               sort: [{ field: "FullName", dir: "desc" }, { field: "DriveDateTimestamp", dir: "desc" }],
               aggregate: [
                   { field: "Distance", aggregate: "sum" },
                   { field: "AmountToReimburse", aggregate: "sum" },
               ]
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
               pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
           },
           scrollable: false,
           dataBinding: function () {
               checkedReports = [];
               $scope.checkAllBox.isChecked = false;
               var temp = $scope.checkboxes.showSubbed;
               $scope.checkboxes = {};
               $scope.checkboxes.showSubbed = temp;
           },
           dataBound: function () {
               this.expandRow(this.tbody.find("tr.k-master-row").first());
           },
           columns: [
               {
                   field: "FullName",
                   title: "Medarbejder"
               }, {
                   field: "Employment.OrgUnit.LongDescription",
                   title: "Org.enhed"
               }, {
                   field: "DriveDateTimestamp",
                   template: function (data) {
                       var m = moment.unix(data.DriveDateTimestamp);
                       return m._d.getDate() + "/" +
                           (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                           m._d.getFullYear();
                   },
                   title: "Dato"
               }, {
                   field: "Purpose",
                   title: "Formål",
               }, {
                   field: "TFCode",
                   title: "Taksttype",
                   template: function (data) {
                       for (var i = 0; i < $scope.rateTypes.length; i++) {
                           if ($scope.rateTypes[i].TFCode == data.TFCode) {
                               return $scope.rateTypes[i].Description;
                           }
                       }
                   }
               }, {
                   title: "Rute",
                   field: "DriveReportPoints",
                   template: function (data) {
                       var tooltipContent = "";
                       if (data.DriveReportPoints != null && data.DriveReportPoints != undefined && data.DriveReportPoints.length > 0) {
                           angular.forEach(data.DriveReportPoints, function (point, key) {
                               if (key != data.DriveReportPoints.length - 1) {
                                   tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town + "<br/>";
                                   gridContent += point.Town + "<br/>";
                               } else {
                                   tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town;
                                   gridContent += point.Town;
                               }
                           });
                       } else {
                           tooltipContent = data.UserComment;
                       }
                       var gridContent = "<i class='fa fa-road fa-2x'></i>";
                       var toolTip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'" + tooltipContent + "'\">" + gridContent + "</div>";
                       var globe = "<div class='inline pull-right margin-right-5' kendo-tooltip k-content=\"'Se rute på kort'\"><a ng-click='showRouteModal(" + data.Id + ")'><i class='fa fa-globe fa-2x'></i></a></div>";
                       if (data.IsOldMigratedReport) {
                           globe = "<div class='inline pull-right margin-right-5' kendo-tooltip k-content=\"'Denne indberetning er overført fra eIndberetning og der kan ikke genereres en rute på et kort'\"><i class='fa fa-circle-thin fa-2x'></i></a></div>";
                       }
                       var roundTrip = "";
                       if (data.IsRoundTrip) {
                           roundTrip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'Ruten er tur/retur'\"><i class='fa fa-exchange fa-2x'></i></div>";
                       }

                       var result = toolTip + roundTrip + globe;
                       var comment = data.UserComment != null ? data.UserComment : "Ingen kommentar angivet";

                       if (data.KilometerAllowance != "Read") {
                           return result;
                       } else {
                           if (data.IsFromApp) {
                               toolTip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'" + tooltipContent + "'\">Indberettet fra mobil app</div>";
                               result = toolTip + globe;
                               return result;
                           } else {
                               return "<div kendo-tooltip k-content=\"'" + comment + "'\">Aflæst manuelt</div>";
                           }
                       }
                   }
               }, {
                   field: "Distance",
                   title: "Km",
                   template: function (data) {
                       return data.Distance.toFixed(2).toString().replace('.', ',') + " km";
                   },
                   footerTemplate: "Total: #= kendo.toString(sum, '0.00').replace('.',',') # km"
               }, {
                   field: "AmountToReimburse",
                   title: "Beløb",
                   template: function (data) {
                       return data.AmountToReimburse.toFixed(2).toString().replace('.', ',') + " kr.";
                   },
                   footerTemplate: "Total: #= kendo.toString(sum, '0.00').replace('.',',') # kr."
               }, {
                   field: "KilometerAllowance",
                   title: "MK",
                   template: function (data) {
                       if (data.IsExtraDistance) {
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
                   title: "Indberettet",
                   template: function (data) {
                       var m = moment.unix(data.CreatedDateTimestamp);
                       return m._d.getDate() + "/" +
                           (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                           m._d.getFullYear();
                   },
               }, {
                   sortable: false,
                   field: "Id",
                   template: function (data) {
                       if (data.ResponsibleLeader.Id == data.PersonId) {
                           return "Indberetning skal godkendes af din leder eller opsat personlig godkender.";
                       }
                       if (data.ResponsibleLeader.Id == $rootScope.CurrentUser.Id) {
                           return "<a ng-click=approveClick(" + data.Id + ")>Godkend</a> | <a ng-click=rejectClick(" + data.Id + ")>Afvis</a> <div class='col-md-1 pull-right'><input type='checkbox' ng-model='checkboxes[" + data.Id + "]' ng-change='rowChecked(" + data.Id + ")'></input></div>";
                       } else {
                           return data.ResponsibleLeader.FullName + " er udpeget som godkender.";
                       }

                   },
                   headerTemplate: "<span>Muligheder</span> <div style='margin: 0' class='col-sm-1 pull-right'><input style='margin: 0' ng-change='checkAllBoxesOnPage()' type='checkbox' ng-model='checkAllBox.isChecked'></input></div>",
                   footerTemplate: "<div class='pull-right fill-width' kendo-toolbar k-options='approveSelectedToolbar'></div>"
               }
           ],
       };

       $scope.checkAllBoxesOnPage = function () {
           /// <summary>
           /// Checks all reports on the current page.
           /// </summary>
           if ($scope.checkAllBox.isChecked) {
               checkedReports = [];
               angular.forEach(allReports, function (value, key) {
                   var repId = value.Id;
                   $scope.checkboxes[repId] = true;
                   checkedReports.push(repId);
               });
           } else {
               angular.forEach(allReports, function (value, key) {
                   var repId = value.Id;
                   $scope.checkboxes[repId] = false;
                   var index = checkedReports.indexOf(repId);
                   checkedReports.splice(index, 1);
               });
           }
       }

       $scope.rowChecked = function (id) {
           /// <summary>
           /// Adds id of the report in the checkedrow to checkedReports.
           /// </summary>
           /// <param name="id"></param>
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
           /// <summary>
           /// Loads initial date filters.
           /// </summary>
           // Set initial values for kendo datepickers.
           var from = new Date();
           from.setDate(from.getDate() - 365);
           $scope.dateContainer.toDate = new Date();
           $scope.dateContainer.fromDate = from;
       }

       $scope.clearName = function () {
           $scope.chosenPerson = "";
       }

       $scope.approveClick = function (id) {
           /// <summary>
           /// Opens approve report modal.
           /// </summary>
           /// <param name="id"></param>
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
               Report.patch({ id: id }, {
                   "Status": "Accepted",
                   "ClosedDateTimestamp": moment().unix(),
                   "ApprovedById": $rootScope.CurrentUser.Id,
               }, function () {
                   $scope.gridContainer.grid.dataSource.read();
               });
           });
       }

       function approveSelectedWithAccountClick() {
           /// <summary>
           /// Opens approve selected reports with different account modal.
           /// </summary>
           if (checkedReports.length == 0) {
               NotificationService.AutoFadeNotification("danger", "", "Ingen indberetninger er markerede!");
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
                       Report.patch({ id: value }, {
                           "Status": "Accepted",
                           "ClosedDateTimestamp": moment().unix(),
                           "AccountNumber": accountNumber,
                           "ApprovedById": $rootScope.CurrentUser.Id,
                       }, function () {
                           $scope.gridContainer.grid.dataSource.read();
                       });
                   });
                   checkedReports = [];
               });
           }
       }

       function approveSelectedClick() {
           /// <summary>
           /// Opens approve selected reports modal.
           /// </summary>
           if (checkedReports.length == 0) {
               NotificationService.AutoFadeNotification("danger", "", "Ingen indberetninger er markerede!");
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
                       Report.patch({ id: value }, {
                           "Status": "Accepted",
                           "ClosedDateTimestamp": moment().unix(),
                           "ApprovedById": $rootScope.CurrentUser.Id,
                       }, function () {
                           $scope.gridContainer.grid.dataSource.read();
                       });
                   });
                   checkedReports = [];
               });
           }
       }

       $scope.showRouteModal = function (routeId) {
           /// <summary>
           /// Opens show route modal.
           /// </summary>
           /// <param name="routeId"></param>
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/HTML/Reports/Modal/ShowRouteModalTemplate.html',
               controller: 'ShowRouteModalController',
               backdrop: "static",
               resolve: {
                   routeId: function () {
                       return routeId;
                   }
               }
           });
       }

       $scope.rejectClick = function (id) {
           /// <summary>
           /// Opens reject report modal.
           /// </summary>
           /// <param name="id"></param>
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
               Report.patch({ id: id }, {
                   "Status": "Rejected",
                   "ClosedDateTimestamp": moment().unix(),
                   "Comment": res.Comment,
                   "ApprovedById": $rootScope.CurrentUser.Id,
               }, function () {
                   $scope.gridContainer.grid.dataSource.read();

               });
           });
       }

       $scope.refreshGrid = function () {
           $scope.gridContainer.grid.dataSource.read();
       }

       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
       $scope.dateContainer = {};

       $scope.loadInitialDates();

       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",

       };

       $scope.person.chosenPerson = "";


       RateType.getAll().$promise.then(function (res) {
           $scope.rateTypes = res;
       });




   }
]);