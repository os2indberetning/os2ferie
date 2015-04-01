angular.module("application").controller("MyPendingReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "$timeout", function ($scope, $modal, $rootScope, Report, $timeout) {

       // Hardcoded personid == 1 until we can get current user from their system.
       var personId = 1;

       // Helper Methods


       $scope.loadReports = function () {
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
                   filter: [{field: "PersonId", operator: "eq", value: personId}],
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
                   pageSizes: [5, 10, 20, 30, 40, 50]
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
                       title: "Kørselsdato"
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
                       title: "Afstand",
                       template: function (data) {
                           return data.Distance.toFixed(2).toString().replace('.', ',') + " Km.";
                       },
                       footerTemplate: "Siden: #= kendo.toString(sum, '0.00').replace('.',',') # Km"
                   }, {
                       field: "AmountToReimburse",
                       title: "Beløb",
                       template: function (data) {
                           return data.AmountToReimburse.toFixed(2).toString().replace('.', ',') + " Dkk.";
                       },
                       footerTemplate: "Siden: #= kendo.toString(sum, '0.00').replace('.',',') # Dkk"
                   }, {
                       field: "CreationDate",
                       template: function (data) {
                           var m = moment.unix(data.CreatedDateTimestamp);
                           return m._d.getDate() + "/" +
                                 (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                  m._d.getFullYear();
                       },
                       title: "Indberetningsdato"
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
           // Set initial values for kendo datepickers.
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

       $scope.deleteClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/MyReports/ConfirmDeleteTemplate.html',
               controller: 'ConfirmDeleteReportController',
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
           // Create a new scope to inject into EditReportController
           var scope = $rootScope.$new();

           // Get the report from the server
           Report.get({ id: id }, function (data) {
               // Set values in the scope.
               scope.purpose = data[0].purpose;
               scope.driveDate = moment.unix(data[0].driveDateTimestamp).format("DD/MM/YYYY");
           });



           var modalInstance = $modal.open({
               scope: scope,
               templateUrl: '/App/MyReports/EditReportModal.html',
               controller: 'EditReportController',
               windowClass: 'full',
           });

           modalInstance.result.then(function (itemId) {

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



       $scope.applyDateFilter = function (fromDateStamp, toDateStamp) {
           var newFilters = [];
           newFilters.push({ field: "PersonId", operator: "eq", value: personId });
           newFilters.push({ field: "DriveDateTimestamp", operator: "gte", value: fromDateStamp });
           newFilters.push({ field: "DriveDateTimestamp", operator: "lte", value: toDateStamp });
           $scope.gridContainer.grid.dataSource.filter(newFilters);
       }

   }
]);