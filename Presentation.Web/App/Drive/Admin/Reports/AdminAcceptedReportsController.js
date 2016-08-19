angular.module("app.drive").controller("AdminAcceptedReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", "BankAccount", "RateType", "Autocomplete", "MkColumnFormatter", "RouteColumnFormatter",
   function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService, BankAccount, RateType, Autocomplete,MkColumnFormatter,RouteColumnFormatter) {

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

       // dates for kendo filter.
       var fromDateFilter = new Date();
       fromDateFilter.setDate(fromDateFilter.getDate() - 30);
       fromDateFilter = $scope.getStartOfDayStamp(fromDateFilter);
       var toDateFilter = $scope.getEndOfDayStamp(new Date());

       $scope.checkboxes = {};
       $scope.checkboxes.showSubbed = false;

       var allReports = [];

       RateType.getAll().$promise.then(function (res) {
           $scope.rateTypes = res;
       });

       $scope.orgUnit = {};
       $scope.orgUnits = [];

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};

       $scope.orgUnits = Autocomplete.orgUnits();
       $scope.people = Autocomplete.allUsers();


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
           var url = "/odata/DriveReports?status=Accepted &getReportsWhereSubExists=true &$expand=DriveReportPoints,ApprovedBy,Employment($expand=OrgUnit)";
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

       /// <summary>
       /// Loads existing accepted reports from backend to kendo grid datasource.
       /// </summary>
       $scope.Reports = {
           autoBind: false,
           dataSource: {
               type: "odata-v4",
               transport: {
                   read: {
                       beforeSend: function(req) {
                           req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                       },
                       url: "/odata/DriveReports?status=Accepted &getReportsWhereSubExists=true &$expand=DriveReportPoints,ApprovedBy,Employment($expand=OrgUnit) &$filter=DriveDateTimestamp ge " + fromDateFilter + " and DriveDateTimestamp le " + toDateFilter,
                       dataType: "json",
                       cache: false
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
           dataBound: function() {
               this.expandRow(this.tbody.find("tr.k-master-row").first());
           },
           columns: [
               {
                   field: "FullName",
                   title: "Medarbejder"
               },
               {
                   field: "Employment.OrgUnit.LongDescription",
                   title: "Org.enhed"
               },
               {
                   field: "DriveDateTimestamp",
                   template: function(data) {
                       var m = moment.unix(data.DriveDateTimestamp);
                       return m._d.getDate() + "/" +
                           (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                           m._d.getFullYear();
                   },
                   title: "Dato"
               },
               {
                   field: "Purpose",
                   title: "Formål",
               },
               {
                   field: "TFCode",
                   title: "Taksttype",
                   template: function(data) {
                       for (var i = 0; i < $scope.rateTypes.length; i++) {
                           if ($scope.rateTypes[i].TFCode == data.TFCode) {
                               return $scope.rateTypes[i].Description;
                           }
                       }
                   }
               },
               {
                   title: "Rute",
                   field: "DriveReportPoints",
                   template: function(data) {
                       return RouteColumnFormatter.format(data);
                   }
               },
               {
                   field: "Distance",
                   title: "Km",
                   template: function(data) {
                       return data.Distance.toFixed(2).toString().replace('.', ',') + " km";
                   },
                   footerTemplate: "Total: #= kendo.toString(sum, '0.00').replace('.',',') # km"
               },
               {
                   field: "AmountToReimburse",
                   title: "Beløb",
                   template: function(data) {
                       return data.AmountToReimburse.toFixed(2).toString().replace('.', ',') + " kr.";
                   },
                   footerTemplate: "Total: #= kendo.toString(sum, '0.00').replace('.',',') # kr."
               },
               {
                   field: "KilometerAllowance",
                   title: "MK",
                   template: function(data) {
                       return MkColumnFormatter.format(data);
                   }
               },
               {
                   field: "FourKmRule",
                   title: "4 km",
                   template: function(data) {
                       if (data.FourKmRule) {
                           return "<i class='fa fa-check'></i>";
                       }
                       return "";
                   }
               },
               {
                   field: "CreatedDateTimestamp",
                   title: "Indberettet",
                   template: function(data) {
                       var m = moment.unix(data.CreatedDateTimestamp);
                       return m._d.getDate() + "/" +
                           (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                           m._d.getFullYear();
                   },
               },
               {
                   field: "ClosedDateTimestamp",
                   title: "Godkendt dato",
                   template: function(data) {
                       var m = moment.unix(data.ClosedDateTimestamp);
                       return m._d.getDate() + "/" +
                           (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                           m._d.getFullYear();
                   }
               },
               {
                   field: "ApprovedBy.FullName",
                   title: "Godkendt af"
               },
               {
                   field: "ProcessedDateTimestamp",
                   title: "Overført til udbetaling",
                   template: function(data) {
                       if (data.ProcessedDateTimestamp != 0 && data.ProcessedDateTimestamp != null && data.ProcessedDateTimestamp != undefined) {
                           var m = moment.unix(data.ProcessedDateTimestamp);
                           return m._d.getDate() + "/" +
                               (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                               m._d.getFullYear();
                       }
                       return "";
                   }
               },
               {
                   title: "Anden kontering",
                   field: "AccountNumber",
                   template: function(data) {
                       if (data.AccountNumber != null && data.AccountNumber != 0 && data.AccountNumber != undefined) {
                           var returnVal = "";
                           angular.forEach($scope.bankAccounts, function(value, key) {
                               if (value.Number == data.AccountNumber) {
                                   returnVal = "Ja " + "<div class='inline' kendo-tooltip k-content=\"'" + value.Description + " - " + value.Number + "'\"> <i class='fa fa-comment-o'></i></div>";
                               }
                           });
                           return returnVal;
                       } else {
                           return "Nej";
                       }
                   }
               }, {
                   field: "Id",
                   title: "Muligheder",
                   template: function(data) {
                       if (data.Status == "Accepted") {
                           return "<a ng-click=rejectClick(" + data.Id + ")>Afvis</a> | <a ng-click=editClick(" + data.Id + ")>Rediger</a>";
                       } else {
                           // Report has already been invoiced.
                           return "Er kørt til løn.";
                       }
                   }
               }
           ],
           scrollable: false
       };

       $scope.editClick = function (id) {
           /// <summary>
           /// Opens edit report modal
           /// </summary>
           /// <param name="id"></param>
           var modalInstance = $modal.open({
               templateUrl: '/App/Drive/MyReports/EditReportTemplate.html',
               controller: 'DrivingController',
               backdrop: "static",
               windowClass: "app-modal-window-full",
               resolve: {
                   ReportId: function () {
                       return id;
                   },
                   adminEditCurrentUser: function () {
                       return Report.getOwner({id : id}).$promise.then(function(res){
                            return Person.GetUserAsCurrentUser({id:res.Id}).$promise.then(function(person){
                                return person;
                            })
                       });
                   }
               }
           });

           $rootScope.editModalInstance = modalInstance;

           modalInstance.result.then(function (res) {
               $scope.gridContainer.grid.dataSource.read();
           });
       }

       /// <summary>
       /// Opens confirm delete accepted report modal
       /// </summary>
       $scope.rejectClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/Drive/Admin/Reports/Modal/ConfirmRejectApprovedReportTemplate.html',
               controller: 'ConfirmRejectApprovedReportModalController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function (res) {
               if(res == undefined){
                   res = "Ingen besked.";
               }
               Report.patch({ id: id, emailText: res }, function () {
                   $scope.gridContainer.grid.dataSource.read();
               });
           });
       }

       $scope.loadInitialDates = function () {
           /// <summary>
           /// Loads initial date filters.
           /// </summary>
           // Set initial values for kendo datepickers.

           initialLoad = 2;

           var from = new Date();
           from.setDate(from.getDate() - 30);

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
               templateUrl: '/App/Drive/Admin/Reports/Modal/ShowRouteModalTemplate.html',
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
           /// <summary>
           /// Refreshes kendo grid datasource.
           /// </summary>

           if ($scope.bankAccounts == undefined) {
               BankAccount.get().$promise.then(function (res) {
                   $scope.bankAccounts = res.value;
               });
           }
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


   }
]);