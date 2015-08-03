angular.module("application").controller("AdminRejectedReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", "RateType", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService, RateType) {

       // Set personId. The value on $rootScope is set in resolve in application.js
       var personId = $rootScope.CurrentUser.Id;

       var allReports = [];

       $scope.tableSortHelp = $rootScope.HelpTexts.TableSortHelp.text;

       RateType.getAll().$promise.then(function (res) {
           $scope.rateTypes = res;
       });

       $scope.getEndOfDayStamp = function (d) {
           var m = moment(d);
           return m.endOf('day').unix();
       }

       $scope.getStartOfDayStamp = function (d) {
           var m = moment(d);
           return m.startOf('day').unix();
       }

       $scope.orgUnitAutoCompleteOptions = {
           filter: "contains",
           select: function (e) {
               $scope.orgUnitChanged();
           }
       }

       $scope.personAutoCompleteOptions = {
           filter: "contains",
           select: function (e) {
               $scope.personChanged();
           }
       };


       // dates for kendo filter.
       var fromDateFilter = new Date();
       fromDateFilter.setDate(fromDateFilter.getDate() - 30);
       fromDateFilter = $scope.getStartOfDayStamp(fromDateFilter);
       var toDateFilter = $scope.getEndOfDayStamp(new Date());

       $scope.checkboxes = {};
       $scope.checkboxes.showSubbed = false;

       $scope.orgUnit = {};
       $scope.orgUnits = [];

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};

       $scope.orgUnits = $rootScope.OrgUnits;
       $scope.people = $rootScope.People;

       $scope.orgUnitChanged = function (item) {
           /// <summary>
           /// Applies OrgUnit filter
           /// </summary>
           /// <param name="item"></param>
           $scope.applyOrgUnitFilter($scope.orgUnit.chosenUnit);
       }


       $scope.showSubsChanged = function () {
           /// <summary>
           /// Updates kendo grid datasource according to getReportsWhereSubExists
           /// </summary>
           $scope.gridContainer.grid.dataSource.transport.options.read.url = "/odata/DriveReports?leaderId=" + personId + "&status=Rejected" + "&getReportsWhereSubExists=" + $scope.checkboxes.showSubbed + " &$expand=Employment($expand=OrgUnit),DriveReportPoints";
           $scope.gridContainer.grid.dataSource.read();
       }

       $scope.applyOrgUnitFilter = function (longDescription) {
           /// <summary>
           /// Applies OrgUnit filter.
           /// </summary>
           /// <param name="longDescription"></param>
           var oldFilters = $scope.gridContainer.grid.dataSource.filter();
           var newFilters = [];


           if (oldFilters == undefined) {
               // If no filters exist, just add the filters.
               if (longDescription != "") {
                   newFilters.push({ field: "Employment.OrgUnit.LongDescription", operator: "eq", value: longDescription });
               }
           } else {
               // If filters already exist then get the old filters, that arent drivedate.
               // Then add the new drivedate filters to these.
               angular.forEach(oldFilters.filters, function (value, key) {
                   if (value.field != "Employment.OrgUnit.LongDescription") {
                       newFilters.push(value);
                   }
               });
               if (longDescription != "") {
                   newFilters.push({ field: "Employment.OrgUnit.LongDescription", operator: "eq", value: longDescription });
               }

           }
           $scope.gridContainer.grid.dataSource.filter(newFilters);
       }

       $scope.applyDateFilter = function (fromDateStamp, toDateStamp) {
           /// <summary>
           /// Applies date filter.
           /// </summary>
           /// <param name="fromDateStamp"></param>
           /// <param name="toDateStamp"></param>

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
           /// <summary>
           /// Applies person filter.
           /// </summary>
           /// <param name="fullName"></param>


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
           /// <summary>
           /// Removes person filter.
           /// </summary>
           $scope.person.chosenPerson = "";
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

       $scope.removeDateFilter = function () {
           /// <summary>
           /// Removes date filter.
           /// </summary>
           $scope.loadInitialDates();
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

       $scope.removeOrgUnitFilter = function () {
           /// <summary>
           /// Removes OrgUnit filter.
           /// </summary>
           $scope.orgUnit.chosenUnit = "";
           var oldFilters = $scope.gridContainer.grid.dataSource.filter();
           if (oldFilters == undefined) {
               return;
           }
           var newFilters = [];
           angular.forEach(oldFilters.filters, function (value, key) {
               if (value.field != "Employment.OrgUnit.LongDescription") {
                   newFilters.push(value);
               }
           });
           $scope.gridContainer.grid.dataSource.filter(newFilters);
       }

       /// <summary>
       /// Loads rejected reports from backend to kendo grid datasource.
       /// </summary>
       $scope.Reports = {
           dataSource: {
               type: "odata",
               transport: {
                   read: {
                       beforeSend: function (req) {
                           req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                       },


                       url: "/odata/DriveReports?status=Rejected &getReportsWhereSubExists=true &$expand=DriveReportPoints,ApprovedBy,Employment($expand=OrgUnit)",
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
               serverPaging: false,
               serverSorting: true,
               filter: [{ field: "DriveDateTimestamp", operator: "gte", value: fromDateFilter }, { field: "DriveDateTimestamp", operator: "lte", value: toDateFilter }],
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
                       angular.forEach(data.DriveReportPoints, function (point, key) {
                           if (key != data.DriveReportPoints.length - 1) {
                               tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town + "<br/>";
                               gridContent += point.StreetName + "<br/>";
                           } else {
                               tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town;
                               gridContent += point.StreetName;
                           }
                       });
                       var gridContent = "<i class='fa fa-road fa-2x'></i>";
                       var toolTip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'" + tooltipContent + "'\">" + gridContent + "</div>";
                       var globe = "<div class='inline pull-right margin-right-5' kendo-tooltip k-content=\"'Se rute på kort'\"><a ng-click='showRouteModal(" + data.Id + ")'><i class='fa fa-globe fa-2x'></i></a></div>";
                       if (data.IsOldMigratedReport) {
                           globe = "<div class='inline pull-right margin-right-5' kendo-tooltip k-content=\"'Denne indberetning er overført fra eIndberetning og der kan ikke genereres en rute på et kort'\"><i class='fa fa-circle-thin fa-2x'></i></a></div>";
                       }
                       var result = toolTip + globe;
                       var comment = data.UserComment != null ? data.UserComment : "Ingen kommentar angivet";

                       if (data.KilometerAllowance != "Read") {
                           return result;
                       } else {
                           if (data.IsFromApp) {
                               toolTip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'" + comment + "'\">Indberettet fra mobil app</div>";
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
                   field: "ClosedDateTimestamp",
                   title: "Afvist dato",
                   template: function (data) {
                       var m = moment.unix(data.ClosedDateTimestamp);
                       var date = m._d.getDate() + "/" +
                           (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                           m._d.getFullYear();

                       return date + "<div class='inline' kendo-tooltip k-content=\"'" + data.Comment + "'\"> <i class='fa fa-comment-o'></i></div>";

                   }
               }, {
                   field: "ApprovedBy.FullName",
                   title: "Afvist af"
               }
           ],
           scrollable: false
       };

       $scope.loadInitialDates = function () {
           // Set initial values for kendo datepickers.

           initialLoad = 2;

           var from = new Date();
           from.setDate(from.getDate() - 30);

           $scope.dateContainer.toDate = new Date();
           $scope.dateContainer.fromDate = from;

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



       // Event handlers

       $scope.clearName = function () {
           $scope.chosenPerson = "";
       }

       $scope.clearClicked = function () {
           $scope.loadInitialDates();
           $scope.removeDateFilter();
           $scope.removePersonFilter();
           $scope.removeOrgUnitFilter();
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

       $scope.showRouteModal = function (routeId) {
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

       $scope.refreshGrid = function () {
           $scope.gridContainer.grid.dataSource.read();
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
   }
]);