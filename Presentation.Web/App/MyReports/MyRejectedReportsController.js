
angular.module("application").controller("MyRejectedReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "$timeout", function ($scope, $modal, $rootScope, Report, $timeout) {

       // Hardcoded personid until we can get current user from their system.
       var personId = 1;

       // Helper Methods






       $scope.updateReports = function (oDataQuery) {
           var and = "and ";
           if (oDataQuery == "") {
               and = "";
           }

           $scope.gridContainer.grid.dataSource.transport.options.read.url = "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected' and PersonId eq " + personId + " " + and + oDataQuery;
           $scope.gridContainer.grid.dataSource.read();
       }





       $scope.loadReports = function () {
           $scope.Reports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },



                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected' and PersonId eq " + personId,
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
                   sort: { field: "DriveDateTimestamp", dir: "desc" }
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
                      title: "Indberettet dato"
                  }, {
                      field: "ClosedDateTimestamp",
                      title: "Afvist dato",
                      template: function (data) {
                          var m = moment.unix(data.ClosedDateTimestamp);
                          var date = m._d.getDate() + "/" +
                                (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                 m._d.getFullYear();
                          return date + "<div kendo-tooltip k-content=\"'" + data.Comment + "'\"><i class='fa fa-comment-o'></i></div>";

                      },
                  }, {
                      field: "ApprovedBy.FullName",
                      title: "Afvist af"
                  }
               ]
           };
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

       $scope.pageSizeChanged = function () {
           $scope.gridContainer.grid.dataSource.pageSize($scope.gridContainer.gridPageSize);
       }

       $scope.searchClicked = function () {
           var from, to;


           from = $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
           to = $scope.getEndOfDayStamp($scope.dateContainer.toDate);

           var q = "DriveDateTimestamp ge " + from + " and DriveDateTimestamp le " + to;
           $scope.updateReports(q);
       }

       var initialLoad = 2;
       $scope.dateChanged = function () {
           // $timeout is a bit of a hack, but it is needed to get the current input value because ng-change is called before ng-model updates.
           $timeout(function () {
               var from, to, and;
               and = " and ";
               from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
               to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDate);


               // Initial load is also a bit of a hack.
               // dateChanged is called twice when the default values for the datepickers are set.
               // This leads to sorting the grid content on load, which is not what we want.
               // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
               if (initialLoad <= 0) {

                   $scope.updateReports(to + and + from);
               }
               initialLoad--;
           }, 0);
       }

       $scope.clearClicked = function () {
           $scope.loadInitialDates();
           $scope.updateReports("");
       }

       // Init


       // Load up the grids.
       $scope.loadReports();


       // Contains references to kendo ui grids.
       $scope.gridContainer = {};
       $scope.dateContainer = {};

       $scope.loadInitialDates();

       // Format for datepickers.
       $scope.dateOptions = {
           format: "dd/MM/yyyy",
       };

       $scope.gridContainer.gridPageSize = 20;
   }
]);