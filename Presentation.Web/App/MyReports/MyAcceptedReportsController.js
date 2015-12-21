angular.module("application").controller("MyAcceptedReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "$timeout", "RateType","MkColumnFormatter", "RouteColumnFormatter", function ($scope, $modal, $rootScope, Report, $timeout, RateType,MkColumnFormatter, RouteColumnFormatter) {

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
       fromDateFilter.setMonth(fromDateFilter.getMonth() - 3);
       fromDateFilter = $scope.getStartOfDayStamp(fromDateFilter);
       var toDateFilter = $scope.getEndOfDayStamp(new Date());

       RateType.getAll().$promise.then(function (res) {
           $scope.rateTypes = res;
       });

       /// <summary>
       /// Loads current user's accepted reports from backend to kendo grid datasource.
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
                       url: "/odata/DriveReports?status=Accepted &$expand=DriveReportPoints,ApprovedBy &$filter=PersonId eq " + personId + " and DriveDateTimestamp ge " + fromDateFilter + " and DriveDateTimestamp le " + toDateFilter,
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
                  field: "CreatedDateTimestamp",
                  template: function (data) {
                      var m = moment.unix(data.CreatedDateTimestamp);
                      return m._d.getDate() + "/" +
                            (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                             m._d.getFullYear();
                  },
                  title: "Indberettet"
              }, {
                  field: "ClosedDateTimestamp",
                  title: "Godkendelsesdato",
                  template: function (data) {
                      var m = moment.unix(data.ClosedDateTimestamp);
                      return m._d.getDate() + "/" +
                            (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                             m._d.getFullYear();
                  },
              }, {
                  field: "ProcessedDateTimestamp",
                  title: "Afsendt til løn",
                  template: function (data) {
                      if (data.ProcessedDateTimestamp != 0 && data.ProcessedDateTimestamp != null && data.ProcessedDateTimestamp != undefined) {
                          var m = moment.unix(data.ProcessedDateTimestamp);
                          return m._d.getDate() + "/" +
                              (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                              m._d.getFullYear();
                      }
                      return "";
                  }
              }, {
                  field: "ApprovedBy.FullName",
                  title: "Godkendt af"
              }
           ],
           scrollable: false
       };

       $scope.clearClicked = function () {
           $scope.loadInitialDates();
           $scope.searchClicked();
       }

       $scope.searchClicked = function () {
           var from = $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
           var to = $scope.getEndOfDayStamp($scope.dateContainer.toDate);
           $scope.gridContainer.grid.dataSource.transport.options.read.url = getDataUrl(from, to);
           $scope.gridContainer.grid.dataSource.read();
       }

       var getDataUrl = function (from, to) {
           var url = "/odata/DriveReports?status=Accepted &$expand=DriveReportPoints,ApprovedBy";
           var filters = " &$filter=PersonId eq " + personId + " and DriveDateTimestamp ge " + from + " and DriveDateTimestamp le " + to;
           var result = url + filters;
           return result;
       }

       $scope.loadInitialDates = function () {
           /// <summary>
           /// Loads initial date filters.
           /// </summary>
           // Set initial values for kendo datepickers.
           var from = new Date();
           from.setMonth(from.getMonth() - 3);

           $scope.dateContainer.toDate = new Date();
           $scope.dateContainer.fromDate = from;
       }

       $scope.refreshGrid = function () {
           /// <summary>
           /// Refreshes kendo grid datasource.
           /// </summary>
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