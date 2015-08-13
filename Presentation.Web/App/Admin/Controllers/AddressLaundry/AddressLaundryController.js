angular.module("application").controller("AddressLaundryController", [
   "$scope", "SmartAdresseSource", "$timeout", "Address", "AddressFormatter", "NotificationService", function ($scope, SmartAdresseSource, $timeout, Address, AddressFormatter, NotificationService) {

       $scope.container = {};
       $scope.SmartAddress = SmartAdresseSource;



       $scope.autoCompleteOptions = {
           filter: "contains"
       };

       $scope.$on('addressLaundryClicked', function (event, mass) {
           Address.GetAutoCompleteDataForCachedAddress().$promise.then(function (res) {
               $scope.autoCompleteDataSource = res.value;
           });
           $scope.container.grid.dataSource.read();
       });

       $scope.addressLocalCopy = [];

       $scope.dirtyAddresses = {
           autoBind: false,
           dataSource: {
               type: "odata-v4",
               transport: {
                   read: {
                       beforeSend: function (req) {
                           req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                       },
                       url: "/odata/Addresses/Service.GetCachedAddresses",
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
                   }
               },
               pageSize: 20,
               serverPaging: true,
               serverSorting: true,
               serverFiltering: true,
               scrollable: false,
           },
           sortable: true,
           pageable: {
               messages: {
                   display: "{0} - {1} af {2} addresser", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                   empty: "Ingen addresser at vise",
                   page: "Side",
                   of: "af {0}", //{0} is total amount of pages
                   itemsPerPage: "addresser pr. side",
                   first: "Gå til første side",
                   previous: "Gå til forrige side",
                   next: "Gå til næste side",
                   last: "Gå til sidste side",
                   refresh: "Genopfrisk"
               },
               pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
           },
           scrollable: false,
           columns: [
               {
                   field: "Description",
                   title: "Beskrivelse",
               },
               {
                   field: "DirtyString",
                   title: "Beskidt adresse"
               }, {
                   title: "Vasket adresse",
                   template: function (data) {
                       if (!data.IsDirty) {
                           return data.StreetName + " " + data.StreetNumber + ", " + data.ZipCode + " " + data.Town;
                       }
                       return "Endnu ikke vasket.";
                   }
               },
               {
                   field: "IsDirty",
                   title: "Vasket",
                   template: function (data) {
                       if (!data.IsDirty) {
                           return '<i class="fa fa-check"></i>';
                       }
                       return "";
                   }
               }, {
                   title: "Ny adresse",
                   width: 300,
                   template: function (data) {
                       return "<input type='text' ng-model='addressLocalCopy[" + data.Id + "]' kendo-auto-complete k-data-text-field=\"'tekst'\" k-data-value-field=\"'tekst'\" k-data-source='SmartAddress' class='form-control fill-width'/>";
                   }
               },
               {
                   title: "Muligheder",
                   template: function (data) {
                       return "<a class='pull-right margin-right-10' ng-click=saveClicked('" + data.Id + "')>Gem</a>"
                   }
               }
           ],
       };

       $scope.saveClicked = function (id) {
           /// <summary>
           /// Attempts to clean address identified by id
           /// </summary>
           /// <param name="id"></param>
           var addr = AddressFormatter.fn($scope.addressLocalCopy[id]);
           if (addr == undefined) {
               NotificationService.AutoFadeNotification("warning", "", "Adressen kunne ikke vaskes.");
               return;
           }
           Address.AttemptCleanCachedAddress({ StreetName: addr.StreetName, StreetNumber: addr.StreetNumber, ZipCode: addr.ZipCode, Town: addr.Town, Id: id }, function () {
               NotificationService.AutoFadeNotification("success", "", "Adressen er vasket.");
               $scope.container.grid.dataSource.read();
           }, function () {
               NotificationService.AutoFadeNotification("warning", "", "Adressen kunne ikke vaskes.");
           });
       }

       $scope.includeCleanChanged = function () {
           /// <summary>
           /// Updates datasource to either show or hide clean addresses.
           /// </summary>
           // Timeout to allow changes to be written to model.
           $timeout(function () {
               $scope.container.grid.dataSource.transport.options.read.url = "/odata/Addresses/Service.GetCachedAddresses?includeCleanAddresses=" + $scope.container.includeClean;
               $scope.container.grid.dataSource.read();
           });
       }

       $scope.descriptionTextChanged = function () {
           /// <summary>
           /// Updates grid filter according to description filter.
           /// </summary>
           $timeout(function () {
               var oldFilters = $scope.container.grid.dataSource.filter();
               var newFilters = [];


               if (oldFilters == undefined) {
                   // If no filters exist, just add the filters.
                   if ($scope.container.descriptionFilter != "") {
                       newFilters.push({ field: "Description", operator: "startswith", value: $scope.container.descriptionFilter });
                   }
               } else {
                   // If filters already exist then get the old filters, that arent ShortDescription.
                   // Then add the new drivedate filters to these.
                   angular.forEach(oldFilters.filters, function (value, key) {
                       if (value.field != "Description") {
                           newFilters.push(value);
                       }
                   });
                   if ($scope.container.descriptionFilter != "") {
                       newFilters.push({ field: "Description", operator: "startswith", value: $scope.container.descriptionFilter });
                   }

               }
               $scope.container.grid.dataSource.filter(newFilters);
           });
       }

       $scope.dirtyStringTextChanged = function () {
           /// <summary>
           /// Updates grid filter according to dirtyString filter
           /// </summary>
           $timeout(function () {
               var oldFilters = $scope.container.grid.dataSource.filter();
               var newFilters = [];


               if (oldFilters == undefined) {
                   // If no filters exist, just add the filters.
                   if ($scope.container.dirtyStringFilter != "") {
                       newFilters.push({ field: "DirtyString", operator: "startswith", value: $scope.container.dirtyStringFilter });
                   }
               } else {
                   // If filters already exist then get the old filters, that arent ShortDescription.
                   // Then add the new drivedate filters to these.
                   angular.forEach(oldFilters.filters, function (value, key) {
                       if (value.field != "DirtyString") {
                           newFilters.push(value);
                       }
                   });
                   if ($scope.container.dirtyStringFilter != "") {
                       newFilters.push({ field: "DirtyString", operator: "startswith", value: $scope.container.dirtyStringFilter });
                   }

               }
               $scope.container.grid.dataSource.filter(newFilters);
           });
       }

       $scope.clearClicked = function () {
           /// <summary>
           /// Clears input fields.
           /// </summary>
           $scope.container.dirtyStringFilter = "";
           $scope.container.descriptionFilter = "";
           $scope.container.grid.dataSource.filter([]);
       }

   }
]);