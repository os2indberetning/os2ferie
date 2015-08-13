angular.module("application").controller("AdminPendingReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", "RateType", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService, RateType) {


       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
       $scope.dateContainer = {};

       $scope.$on('reportsClicked', function (event, mass) {
           $timeout(function () {
               $scope.gridContainer.grid.dataSource.read();
           },0);
       });

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};
       $scope.orgUnit = {};
       $scope.orgUnits = [];

       $scope.tableSortHelp = $rootScope.HelpTexts.TableSortHelp.text;

       // Set personId. The value on $rootScope is set in resolve in application.js
       var personId = $rootScope.CurrentUser.Id;

       $scope.orgUnits = $rootScope.OrgUnits;
       $scope.people = $rootScope.People;

       $scope.searchClicked = function () {
           var from = $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
           var to = $scope.getEndOfDayStamp($scope.dateContainer.toDate);
           $scope.gridContainer.grid.dataSource.filter(getFilters(from, to, $scope.person.chosenPerson, $scope.orgUnit.chosenUnit));
       }

       var getFilters = function (from, to, fullName, longDescription) {
           var newFilters = [];
           newFilters.push({ field: "DriveDateTimestamp", operator: "ge", value: from });
           newFilters.push({ field: "DriveDateTimestamp", operator: "le", value: to });
           if (fullName != undefined && fullName != "") {
               newFilters.push({ field: "FullName", operator: "eq", value: fullName });
           }
           if (longDescription != undefined && longDescription != "") {
               newFilters.push({ field: "Employment.OrgUnit.LongDescription", operator: "eq", value: longDescription });
           }
           return newFilters;
       }

       RateType.getAll().$promise.then(function (res) {
           $scope.rateTypes = res;
       });

       $scope.orgUnitAutoCompleteOptions = {
           filter: "contains",
       }

       $scope.personAutoCompleteOptions = {
           filter: "contains",
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

       var allReports = [];

       // Helper Methods




       /// <summary>
       /// Loads pending reports from backend to kendo grid datasource
       /// </summary>
       $scope.Reports = {
           autoBind: false,
           dataSource: {
               type: "odata-v4",
               transport: {
                   read: {
                       beforeSend: function (req) {
                           req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                       },
                       url: "/odata/DriveReports?status=Pending &getReportsWhereSubExists=true &$expand=DriveReportPoints,ResponsibleLeader,Employment($expand=OrgUnit)",
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
               pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200],
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
                               gridContent += point.Town + "<br/>";
                           } else {
                               tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town;
                               gridContent += point.Town;
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
                   field: "ResponsibleLeader.FullName",
                   title: "Godkender"
               }
           ],
           scrollable: false,
       };

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





       // Event handlers



       $scope.clearName = function () {
           $scope.chosenPerson = "";
       }

       $scope.clearClicked = function () {
           /// <summary>
           /// Clears filters.
           /// </summary>
           $scope.loadInitialDates();
           $scope.person.chosenPerson = "";
           $scope.orgUnit.chosenUnit = "";
           $scope.searchClicked();
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

       $scope.refreshGrid = function () {
           $scope.gridContainer.grid.dataSource.read();
       }




       $scope.loadInitialDates();



       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",

       };

       $scope.personChanged = function (item) {

       }

       $scope.person.chosenPerson = "";






   }
]);