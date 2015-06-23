angular.module("application").controller("MyPendingReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "$timeout", "Person", function ($scope, $modal, $rootScope, Report, $timeout, Person) {

       // Set personId. The value on $rootScope is set in resolve in application.js
       var personId = $rootScope.CurrentUser.Id;

       $scope.tableSortHelp = $rootScope.HelpTexts.TableSortHelp.text;

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

       $scope.loadReports = function () {
           /// <summary>
           /// Loads current user's pending reports from backend to kendo grid datasource.
           /// </summary>
           $scope.Reports = {
               dataSource: {
                   type: "odata-v4",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/DriveReports?status=Pending &$expand=DriveReportPoints,ResponsibleLeader",
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
                   pageSize: 20,
                   serverPaging: true,
                   serverAggregates: false,
                   serverSorting: true,
                   serverFiltering: true,
                   filter: [{ field: "PersonId", operator: "eq", value: personId }, { field: "DriveDateTimestamp", operator: "gte", value: fromDateFilter }, { field: "DriveDateTimestamp", operator: "lte", value: toDateFilter }],
                   sort: { field: "DriveDateTimestamp", dir: "desc" },
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
                   },
                   pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
               },
               dataBound: function () {
                   this.expandRow(this.tbody.find("tr.k-master-row").first());
               },
               columns: [
                   {
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
                       template: function (data) {
                           if (data.Comment != "") {
                               return data.Purpose + "<button kendo-tooltip k-position=\"'right'\" k-content=\"'" + data.Comment + "'\" class=\"transparent-background pull-right no-border\"><i class=\"fa fa-comment-o\"></i></button>";
                           }
                           return data.Purpose;

                       },
                       title: "Formål"
                   }, {
                       title: "Rute",
                       field: "DriveReportPoints",
                       template: function (data) {
                           var tooltipContent = "";
                           angular.forEach(data.DriveReportPoints, function (point, key) {
                               if (key != data.DriveReportPoints.length - 1) {
                                   tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town + " <br/> ";
                               } else {
                                   tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town;
                               }
                           });
                           var gridContent = "<i class='fa fa-road fa-2x'></i>";
                           var toolTip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'" + tooltipContent + "'\">" + gridContent + "</div>";
                           var globe = "<div class='inline pull-right margin-right-5' kendo-tooltip k-content=\"'Se rute på kort'\"><a ng-click='showRouteModal(" + data.Id + ")'><i class='fa fa-globe fa-2x'></i></a></div>";
                           var result = toolTip + globe;


                           if (data.KilometerAllowance != "Read") {
                               return result;
                           } else {
                               if (data.IsFromApp) {
                                   toolTip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'" + data.UserComment + "'\">Indberettet fra mobil app</div>";
                                   result = toolTip + globe;
                                   return result;
                               } else {
                                   return "<div kendo-tooltip k-content=\"'" + data.UserComment + "'\">Aflæst manuelt</div>";
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
                       field: "CreationDate",
                       template: function (data) {
                           var m = moment.unix(data.CreatedDateTimestamp);
                           return m._d.getDate() + "/" +
                                 (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                  m._d.getFullYear();
                       },
                       title: "Indberettet"
                   }, {
                       field: "ResponsibleLeader.FullName",
                       title: "Godkender"
                   }, {
                       field: "Id",
                       template: "<a ng-click=deleteClick(${Id})>Slet</a> | <a ng-click=editClick(${Id})>Rediger</a>",
                       title: "Muligheder"
                   }
               ],
               scrollable: false,
           };
       }



       $scope.loadInitialDates = function () {
           /// <summary>
           /// Sets initial date filters.
           /// </summary>
           // Set initial values for kendo datepickers.
           var from = new Date();
           from.setDate(from.getDate() - 365);

           $scope.dateContainer.toDate = new Date();
           $scope.dateContainer.fromDate = from;

       }



       // Event handlers

       $scope.deleteClick = function (id) {
           /// <summary>
           /// Opens delete report modal
           /// </summary>
           /// <param name="id"></param>
           var modalInstance = $modal.open({
               templateUrl: '/App/MyReports/ConfirmDeleteTemplate.html',
               controller: 'ConfirmDeleteReportController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function (res) {
               Report.delete({ id: id }, function () {
                   $scope.gridContainer.grid.dataSource.read();
               });
           });
       }

       $scope.editClick = function (id) {
           /// <summary>
           /// Opens edit report modal
           /// </summary>
           /// <param name="id"></param>
           var modalInstance = $modal.open({
               templateUrl: '/App/MyReports/EditReportTemplate.html',
               controller: 'DrivingController',
               backdrop: "static",
               windowClass: "app-modal-window-full",
               resolve: {
                   ReportId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function (res) {
               $scope.gridContainer.grid.dataSource.read();
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

       $scope.clearClicked = function () {
           $scope.gridContainer.grid.dataSource.filter([{ field: "PersonId", operator: "eq", value: personId }]);
           $scope.loadInitialDates();
       }


       // Load up the grid.
       $scope.loadReports();


       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
       $scope.dateContainer = {};

       $scope.loadInitialDates();

       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",
       };

       $scope.refreshGrid = function () {
           /// <summary>
           /// Refreshes kendo grid datasource.
           /// </summary>
           $scope.gridContainer.grid.dataSource.read();
       }


       $scope.applyDateFilter = function (fromDateStamp, toDateStamp) {
           /// <summary>
           /// Applies date filters.
           /// </summary>
           /// <param name="fromDateStamp"></param>
           /// <param name="toDateStamp"></param>
           var newFilters = [];
           newFilters.push({ field: "PersonId", operator: "eq", value: personId });
           newFilters.push({ field: "DriveDateTimestamp", operator: "gte", value: fromDateStamp });
           newFilters.push({ field: "DriveDateTimestamp", operator: "lte", value: toDateStamp });
           $scope.gridContainer.grid.dataSource.filter(newFilters);
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



   }
]);