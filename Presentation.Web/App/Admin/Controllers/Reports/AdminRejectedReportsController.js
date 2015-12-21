angular.module("application").controller("AdminRejectedReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", "RateType","Autocomplete","MkColumnFormatter", "RouteColumnFormatter", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService, RateType, Autocomplete,MkColumnFormatter,RouteColumnFormatter) {

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
               $scope.orgUnit.chosenId = this.dataItem(e.item.index()).Id;
           }
       }

       $scope.personAutoCompleteOptions = {
           filter: "contains",
           select: function (e) {
               $scope.person.chosenId = this.dataItem(e.item.index()).Id;
           }
       };

       $scope.searchClicked = function () {
           var from = $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
           var to = $scope.getEndOfDayStamp($scope.dateContainer.toDate);
           $scope.gridContainer.grid.dataSource.transport.options.read.url = getDataUrl(from, to, $scope.person.chosenPerson, $scope.orgUnit.chosenUnit);
           $scope.gridContainer.grid.dataSource.read();
       }

       var getDataUrl = function (from, to, fullName, longDescription) {
           var url = "/odata/DriveReports?status=Rejected &getReportsWhereSubExists=true &$expand=DriveReportPoints,ApprovedBy,Employment($expand=OrgUnit)";
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

        $scope.orgUnits = Autocomplete.orgUnits();
        $scope.people = Autocomplete.allUsers();

       /// <summary>
       /// Loads rejected reports from backend to kendo grid datasource.
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


                       url: "/odata/DriveReports?status=Rejected &getReportsWhereSubExists=true &$expand=DriveReportPoints,ApprovedBy,Employment($expand=OrgUnit) &$filter=DriveDateTimestamp ge " + fromDateFilter + " and DriveDateTimestamp le " + toDateFilter,
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
                       return RouteColumnFormatter.format(data);
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
                       return MkColumnFormatter.format(data);
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

       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
       $scope.dateContainer = {};

       $scope.loadInitialDates();

       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",
       };

   }
]);