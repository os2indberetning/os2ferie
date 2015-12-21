angular.module("application").controller("AcceptedReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", "BankAccount", "RateType", "Autocomplete","MkColumnFormatter", "RouteColumnFormatter",
   function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService, BankAccount, RateType, Autocomplete,MkColumnFormatter, RouteColumnFormatter) {

       // Set personId. The value on $rootScope is set in resolve in application.js
       var personId = $rootScope.CurrentUser.Id;
       $scope.isLeader = $rootScope.CurrentUser.IsLeader;

       $scope.tableSortHelp = $rootScope.HelpTexts.TableSortHelp.text;

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

       RateType.getAll().$promise.then(function (res) {
           $scope.rateTypes = res;
       });

       // dates for kendo filter.
       var fromDateFilter = new Date();
       fromDateFilter.setMonth(fromDateFilter.getMonth() - 3);
       fromDateFilter = $scope.getStartOfDayStamp(fromDateFilter);
       var toDateFilter = $scope.getEndOfDayStamp(new Date());

       $scope.checkboxes = {};
       $scope.checkboxes.showSubbed = false;

       var allReports = [];

       $scope.orgUnit = {};
       $scope.orgUnits = Autocomplete.orgUnits();

       $scope.people = Autocomplete.activeUsers();
       $scope.person = {};


       $scope.showSubsChanged = function () {
           /// <summary>
           /// Updates datasource accoridng to getReportsWhereSubExists
           /// </summary>
           $scope.searchClicked();
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

       $scope.searchClicked = function () {
           var from = $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
           var to = $scope.getEndOfDayStamp($scope.dateContainer.toDate);
           $scope.gridContainer.grid.dataSource.transport.options.read.url = getDataUrl(from, to, $scope.person.chosenPerson, $scope.orgUnit.chosenUnit);
           $scope.gridContainer.grid.dataSource.read();
       }

       var getDataUrl = function (from, to, fullName, longDescription) {
           var url = "/odata/DriveReports?leaderId=" + personId + "&status=Accepted" + "&getReportsWhereSubExists=" + $scope.checkboxes.showSubbed + " &$expand=Employment($expand=OrgUnit),DriveReportPoints";
           var filters = "&$filter=DriveDateTimestamp ge " + from + " and DriveDateTimestamp le " + to;
           var leaderFilter = " and ResponsibleLeaderId eq " + $scope.CurrentUser.Id;
           if ($scope.checkboxes.showSubbed) {
               leaderFilter = " and (ResponsibleLeaderId eq " + $scope.CurrentUser.Id + " or ActualLeaderId eq " + $scope.CurrentUser.Id + ")";
           }
           if (fullName != undefined && fullName != "") {
               filters += " and PersonId eq " + $scope.person.chosenId;
           }
           if (longDescription != undefined && longDescription != "") {
               filters += " and Employment/OrgUnitId eq " + $scope.orgUnit.chosenId;
           }
           filters += leaderFilter;
           var result = url + filters;
           return result;
       }

       $scope.reports = {
           autoBind: false,
           dataSource: {

               sort: [{ field: "FullName", dir: "desc" }, { field: "DriveDateTimestamp", dir: "desc" }],
               type: "odata-v4",
               transport: {
                   read: {
                       beforeSend: function (req) {
                           req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                       },
                       url: "/odata/DriveReports?status=Accepted &$expand=Employment($expand=OrgUnit),DriveReportPoints &$filter=DriveDateTimestamp ge " + fromDateFilter + " and DriveDateTimestamp le " + toDateFilter + " and ResponsibleLeaderId eq " + $scope.CurrentUser.Id,
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
                       return data.value;

                   },
                   total: function (data) {
                       return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                   },
               },
               pageSize: 50,
               serverPaging: true,
               serverAggregates: false,
               serverSorting: true,
               aggregate: [
                   { field: "Distance", aggregate: "sum" },
                   { field: "AmountToReimburse", aggregate: "sum" }
               ]
           },
           sortable: { mode: "multiple" },
           scrollable: false,
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
               },
               {
                   field: "ClosedDateTimestamp",
                   title: "Godkendt dato",
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
                   title: "Anden kontering",
                   field: "AccountNumber",
                   template: function (data) {
                       if (data.AccountNumber != null && data.AccountNumber != 0 && data.AccountNumber != undefined) {
                           var returnVal = "";
                           angular.forEach($scope.bankAccounts, function (value, key) {
                               if (value.Number == data.AccountNumber) {
                                   returnVal = "Ja " + "<div class='inline' kendo-tooltip k-content=\"'" + value.Description + " - " + value.Number + "'\"> <i class='fa fa-comment-o'></i></div>";
                               }
                           });
                           return returnVal;
                       } else {
                           return "Nej";
                       }
                   }
               }
           ],
       };

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

       // Event handlers

       $scope.clearName = function () {
           $scope.chosenPerson = "";
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



       // Init


       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
       $scope.dateContainer = {};


       $scope.refreshGrid = function () {
           $scope.gridContainer.grid.dataSource.read();
       }

       $scope.loadInitialDates();

       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",
       };

       BankAccount.get().$promise.then(function (res) {
           $scope.bankAccounts = res.value;
       });
   }
]);