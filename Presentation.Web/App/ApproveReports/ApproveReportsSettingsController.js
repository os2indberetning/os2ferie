angular.module("application").controller("ApproveReportsSettingsController", [
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
                           url: "odata/Substitutes/Service.Substitute?$expand=OrgUnit,Sub",
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
                   }
               },
               sortable: true,
               pageable: true,
               dataBound: function () {
                   this.expandRow(this.tbody.find("tr.k-master-row").first());
               },
               columns: [{
                   field: "Sub.FullName",
                   title: "Stedfortræder"
               }, {
                   field: "StartDateTimestamp",
                   title: "Fra",
                   template: "#= kendo.toString(new Date(StartDateTimestamp*1000), 'MM/dd/yyyy') #"
               }, {
                   field: "EndDateTimestamp",
                   title: "Til",
                   template: "#= kendo.toString(new Date(EndDateTimestamp*1000), 'MM/dd/yyyy') #"
               }, {
                   field: "OrgUnit.ShortDescription",
                   title: "Organisation"
               }, {
                   title: "Muligheder",
                   template: "<a class='k-button' ng-click='openEditSubstitute(${Id})'>Rediger</a><a class='k-button' ng-click='deleteSubstitute(Id)'>Slet</a>"
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
                           url: "odata/Substitutes/Service.Personal?$expand=OrgUnit,Sub,Leader",
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
                   field: "Sub.FullName",
                   title: "Godkender"
               }, {
                   field: "Leader.FullName",
                   title: "Afviger"
               }, {
                   field: "Title", // Kendo grid doesn't support arrays
                   title: "Ansatte"
               }, {
                   field: "StartDateTimestamp",
                   title: "Fra",
                   template: "#= kendo.toString(new Date(StartDateTimestamp*1000), 'MM/dd/yyyy') #"
               }, {
                   field: "EndDateTimestamp",
                   title: "Til",
                   template: "#= kendo.toString(new Date(EndDateTimestamp*1000), 'MM/dd/yyyy') #"
               }, {
                   title: "Muligheder",
                   template: "<a class='k-button' ng-click='openEditApprover(${Id})'>Rediger</a><a class='k-button' ng-click='deleteApprover(Id)'>Slet</a>"
               }]
           };
       }

       $scope.loadGrids();

       $scope.openEditSubstitute = function (id) {
           var modalInstance = $modal.open({
               templateUrl: 'App/ApproveReports/Modals/editSubstituteModal.html',
               controller: 'EditSubstituteModalInstanceController',
               backdrop: 'static',
               size: 'lg',
               resolve: {
                   persons: function () {
                       return $scope.persons;
                   },
                   orgUnits: function () {
                       return $scope.orgUnits;
                   },
                   leader: function () {
                       return $scope.currentPerson;
                   },
                   id: function () {
                       return id;
                   }
               }
           });
       }

       $scope.openEditApprover = function(id) {
           var modalInstance = $modal.open({
               templateUrl: 'App/ApproveReports/Modals/editApproverModal.html',
               controller: 'EditApproverModalInstanceController',
               backdrop: 'static',
               size: 'lg',
               resolve: {
                   persons: function () {
                       return $scope.persons;
                   },
                   orgUnits: function () {
                       return $scope.orgUnits;
                   },
                   leader: function () {
                       return $scope.currentPerson;
                   },
                   id: function() {
                       return id;
                   }
               }
           });
       }

       $scope.createNewApprover = function () {
           var modalInstance = $modal.open({
               templateUrl: 'App/ApproveReports/Modals/newApproverModal.html',
               controller: 'NewApproverModalInstanceController',
               backdrop: 'static',
               size: 'lg',
               resolve: {
                   persons: function () {
                       return $scope.persons;
                   },
                   orgUnits: function () {
                       return $scope.orgUnits;
                   },
                   leader: function () {
                       return $scope.currentPerson;
                   }
               }
           });

           modalInstance.result.then(function () {
               $scope.loadGrids();
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
                   },
                   orgUnits: function () {
                       return $scope.orgUnits;
                   },
                   leader: function () {
                       return $scope.currentPerson;
                   }
               }
           });

           modalInstance.result.then(function () {
               $scope.loadGrids();
           }, function () {

           });
       };
   }
]);