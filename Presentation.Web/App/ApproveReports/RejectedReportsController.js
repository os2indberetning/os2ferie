angular.module("application").controller("RejectedReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService) {



       var queryOptions = { dateQuery: "", personQuery: "" };


       $scope.checkboxes = {};

       var checkedReports = [];

       var allReports = [];

       // Helper Methods







       $scope.updateReports = function () {
           var dateAnd = "and ";
           if (queryOptions.dateQuery == "") {
               dateAnd = "";
           }
           var dateQuery = dateAnd + queryOptions.dateQuery;

           var personAnd = "and ";
           if (queryOptions.personQuery == "") {
               personAnd = "";
           }
           var personQuery = personAnd + queryOptions.personQuery;

           var query = dateQuery + personQuery;

           var url = "/odata/DriveReports?$expand=Employment &$filter=Status eq Core.DomainModel.ReportStatus'Rejected' " + query;



           $scope.gridContainer.grid.dataSource.transport.options.read.url = url;
           $scope.gridContainer.grid.dataSource.read();
       }



       $scope.filterReportsByLeaderOrg = function (orgs, data, leaderOrgId) {
           var orgUnits = {};

           var resultSet = [];

           angular.forEach(orgs.value, function (value, key) {
               orgUnits[value.Id] = value;
           });

           angular.forEach(data.value, function (value, key) {
               var repOrg = orgUnits[value.Employment.OrgUnitId];

               if (orgUnits[leaderOrgId].Level == repOrg.Level && orgUnits[leaderOrgId].Id == repOrg.Id) {

                   resultSet.push(value);
               } else if (orgUnits[leaderOrgId].Level < repOrg.Level) {
                   while (orgUnits[leaderOrgId].Level < repOrg.Level) {
                       repOrg = orgUnits[repOrg.ParentId];
                   }
                   if (repOrg.Id == orgUnits[leaderOrgId].Id) {
                       resultSet.push(value);
                   }
               }

           });
           return resultSet;
       }



       $scope.loadReports = function () {
           $scope.reports = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected'&$expand=Employment",
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

                           var leaderOrgId = 1;
                           var resultSet = [];

                           var orgs = OrgUnit.get();

                           orgs.$promise.then(function (res) {


                               resultSet = $scope.filterReportsByLeaderOrg(orgs, data, leaderOrgId);

                               $scope.gridContainer.grid.dataSource.data(resultSet);
                               $scope.gridContainer.grid.refresh();

                           });
                           return resultSet;

                       },
                       total: function (data) {
                           return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                       }
                   },
                   pageSize: 5,
                   serverPaging: false,
                   serverSorting: true
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
                   }
               },
               dataBound: function () {
                   this.expandRow(this.tbody.find("tr.k-master-row").first());
               },
               columns: [
                                  {
                                      field: "Fullname",
                                      title: "Navn"
                                  }, {
                                      field: "CreationDate",
                                      template: function (data) {
                                          var m = moment.unix(data.CreatedDateTimestamp);
                                          return m._d.getDate() + "/" +
                                                (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                                 m._d.getFullYear();
                                      },
                                      title: "Indberettet den"
                                  }, {
                                      field: "DriveDateTimestamp",
                                      template: function (data) {
                                          var m = moment.unix(data.DriveDateTimestamp);
                                          return m._d.getDate() + "/" +
                                              (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                              m._d.getFullYear();
                                      },
                                      title: "Kørselsdato"
                                  }, {
                                      field: "Id",
                                      template: function (data) {
                                          if (data.Comment != "") {
                                              return data.Purpose + "<button kendo-tooltip k-position=\"'right'\" k-content=\"'" + data.Comment + "'\" class=\"transparent-background pull-right no-border\"><i class=\"fa fa-comment-o\"></i></button>";
                                          }
                                          return data.Purpose;

                                      },
                                      title: "Formål"

                                  }, {
                                      field: "AmountToReimburse",
                                      title: "Beløb"
                                  }, {
                                      field: "Distance",
                                      title: "Afstand"
                                  }
               ]
           };
       }

       $scope.loadInitialDates = function () {
           // Set initial values for kendo datepickers.
           $scope.dateContainer.toDate = new Date();
           $scope.dateContainer.fromDate = new Date();
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
           $scope.gridContainer.grid.dataSource.pageSize(Number($scope.gridContainer.gridPageSize));
       }

       $scope.clearName = function () {
           $scope.chosenPerson = "";
       }

       $scope.clearClicked = function () {
           queryOptions.dateQuery = "";
           queryOptions.personQuery = "";
           $scope.person.chosenPerson = "";
           $scope.updateReports();
       }

       $scope.dateChanged = function () {

           //TODO: Shit doesnt work if the input field is left empty. It yields NaN and gives no results.

           // $timeout is a bit of a hack, but it is needed to get the current input value because ng-change is called before ng-model updates.
           $timeout(function () {
               var from, to, and;
               and = " and ";
               from = "DriveDateTimestamp ge " + $scope.getStartOfDayStamp($scope.dateContainer.fromDate);
               to = "DriveDateTimestamp le " + $scope.getEndOfDayStamp($scope.dateContainer.toDate);
               queryOptions.dateQuery = from + and + to;
               $scope.updateReports();
           }, 0);


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


       // Load up the grids.
       $scope.loadReports();

       $scope.personChanged = function (item) {
           queryOptions.personQuery = "PersonId eq " + item.Id;
           $scope.updateReports();
       }

       // Load people for auto-complete textbox
       $scope.people = [];
       $scope.person = {};

       // Set initial value for grid pagesize
       $scope.gridContainer.gridPageSize = 5;

       Person.getAll().$promise.then(function (res) {
           angular.forEach(res.value, function (value, key) {
               $scope.people.push({ Id: value.Id, FullName: value.FullName });
           });
       });




   }
]);