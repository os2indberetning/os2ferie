angular.module("application").controller("LeaderSettingsController", [
   "$scope", "OrgUnit", "Person", "$modal", function ($scope, OrgUnit, Person, $modal) {
       $scope.grindContainer = [];
       $scope.collapseSubtitute = false;
       $scope.collapsePersonalApprover = false;
       $scope.orgUnits = [];
       $scope.persons = [];
       $scope.currentPerson = {};

       Person.get({ id: 1 }, function (data) {
           $scope.currentPerson = data;
       });

       Person.getAll(function (data) {
           $scope.persons = data.value;
           console.log($scope.persons);
       });

       $scope.substituteOrgUnit = "";

       OrgUnit.get(function (data) {
           $scope.orgUnits = data.value;
       });

       $scope.loadGrids = function () {
           $scope.substitutes = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "odata/Substitutes()",//"?$expand=OrgUnit and Sub",
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
               },
               sortable: true,
               pageable: true,
               dataBound: function () {
                   this.expandRow(this.tbody.find("tr.k-master-row").first());
               },
               columns: [{
                   field: "Id",
                   title: "Stedfortræder"
               }, {
                   field: "StartDateTimestamp",
                   title: "Fra"
               }, {
                   field: "EndDateTimestamp",
                   title: "Til"
               }, {
                   field: "Id",
                   title: "Organisation"
               }, {
                   field: "Id",
                   title: "Muligheder"
               }]
           };

           $scope.personalApprovers = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "odata/Substitutes()",
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
               },
               sortable: true,
               pageable: true,
               dataBound: function () {
                   this.expandRow(this.tbody.find("tr.k-master-row").first());
               },
               columns: [{
                   field: "Id",
                   title: "Godkender"
               }, {
                   field: "Id",
                   title: "Afviger"
               }, {
                   field: "Id",
                   title: "Ansatte"
               }, {
                   field: "StartDateTimestamp",
                   title: "Fra"
               }, {
                   field: "EndDateTimestamp",
                   title: "Til"
               }, {
                   field: "Title",
                   title: "Muligheder"
               }]
           };
       }

       $scope.loadGrids();

       $scope.createNewApprover = function () {
           var modalInstance = $modal.open({
               templateUrl: 'App/ApproveReports/Modals/newApproverModal.html',
               controller: 'NewApproverModalInstanceController',
               backdrop: 'static',
               size: 'lg',
               resolve: {
                   persons: function () {
                       return $scope.persons;
                   }
               }
           });

           modalInstance.result.then(function (selectedItem) {
               $scope.selected = selectedItem;
           }, function () {
               
           });
       };

       $scope.createNewSubstitute = function () {
           var modalInstance = $modal.open({
               templateUrl: 'App/ApproveReports/Modals/newSubstituteModal.html',
               controller: 'NewSubstituteModalInstanceController',
               backdrop: 'static',
               size: 'lg',
               resolve: {
                   persons: function () {
                       return $scope.persons;
                   }
               }
           });

           modalInstance.result.then(function (selectedItem) {
               $scope.selected = selectedItem;
           }, function () {
               
           });
       };
   }
]);