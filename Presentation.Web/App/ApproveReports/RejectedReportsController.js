angular.module("application").controller("RejectedReportsController", [
   "$scope", "$modal", "$rootScope", "Report", "OrgUnit", "Person", "$timeout", "NotificationService", function ($scope, $modal, $rootScope, Report, OrgUnit, Person, $timeout, NotificationService) {



       var queryOptions = { dateQuery: "", personQuery: "" };

       var allReports = [];


       $scope.orgUnit = {};
       $scope.orgUnits = [];

       OrgUnit.get().$promise.then(function (res) {
           $scope.orgUnits = res.value;
       });

       $scope.orgUnitChanged = function (item) {
           var filter = [];
           filter.push({ field: "Employment.OrgUnit.ShortDescription", operator: "contains", value: $scope.orgUnit.chosenUnit });
           $scope.gridContainer.grid.dataSource.filter(filter);
       }

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

           var url = "/odata/DriveReports?$expand=Employment($expand=OrgUnit),DriveReportPoints &$filter=Status eq Core.DomainModel.ReportStatus'Rejected' " + query;



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
                   type: "odata-v4",
                   transport: {
                       read: {
                           url: "/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected'&$expand=Employment($expand=OrgUnit),DriveReportPoints",
                       },
                   },
                   schema: {
                       data: function (data) {

                           allReports = data.value;

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
                   },
                   pageSize: 20,
                   serverPaging: false,
                   serverSorting: true,
                   sort: [{ field: "FullName", dir: "desc" }, { field: "DriveDateTimestamp", dir: "desc" }]
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
                   pageSizes: [5, 10, 20, 30, 40, 50]
               },
               dataBound: function () {
                   $scope.getCurrentPageSums();
                   $scope.getAllPagesSums();
                   this.expandRow(this.tbody.find("tr.k-master-row").first());
               },
               columns: [
               {
                   field: "FullName",
                   title: "Navn"
               }, {
                   field: "Employment.OrgUnit.ShortDescription",
                   title: "Organisationsenhed"
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
                   field: "Purpose",
                   title: "Formål",
               }, {
                   title: "Rute",
                   field: "DriveReportPoints",
                   template: function (data) {
                       var tooltipContent = "";
                       var gridContent = "";
                       angular.forEach(data.DriveReportPoints, function (point, key) {
                           if (key != data.DriveReportPoints.length - 1) {
                               tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town + "<br/>";
                               gridContent += point.StreetName + "<br/>";
                           } else {
                               tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town;
                               gridContent += point.StreetName;
                           }
                       });
                       var result = "<div kendo-tooltip k-content=\"'" + tooltipContent + "'\">" + gridContent + "</div>";

                       if (data.KilometerAllowance != "Read") {
                           return result;
                       } else {
                           return data.UserComment;
                       }
                   }
               }, {
                   field: "Distance",
                   title: "Km",
                   template: function (data) {
                       return data.Distance.toFixed(2).toString().replace('.', ',') + " Km";
                   },
                   footerTemplate: "Siden: {{currentPageDistanceSum}} KM <br/> Total: {{allPagesDistanceSum}} KM"
               }, {
                   field: "AmountToReimburse",
                   title: "Beløb",
                   template: function (data) {
                       return data.AmountToReimburse.toFixed(2).toString().replace('.', ',') + " Dkk.";
                   },
                   footerTemplate: "Siden: {{currentPageAmountSum}} Dkk. <br/> Total: {{allPagesAmountSum}} Dkk."
               }, {
                   field: "KilometerAllowance",
                   title: "Merkørsel",
                   template: function (data) {
                       if (data.KilometerAllowance == "CalculatedWithoutExtraDistance") {
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
                   title: "Indberetningsdato",
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

                               return date +  "<div class='inline' kendo-tooltip k-content=\"'" + data.Comment + "'\"> <i class='fa fa-comment-o'></i></div>";

                       }
                   }
               ],
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
           $scope.loadInitialDates();
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
                   queryOptions.dateQuery = from + and + to;
                   $scope.updateReports();
               }
               initialLoad--;
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
       $scope.gridContainer.gridPageSize = 20;

       Person.getAll().$promise.then(function (res) {
           angular.forEach(res.value, function (value, key) {
               $scope.people.push({ Id: value.Id, FullName: value.FullName });
           });
       });




   }
]);