angular.module("application").controller("StandardAddressController", [
   "$scope", "$modal", "StandardAddress", "AddressFormatter", function ($scope, $modal, StandardAddress, AddressFormatter) {

       $scope.loadAddresses = function () {
           $scope.addresses = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/Addresses",
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
                   pageSize: 5,
                   serverPaging: false,
                   serverSorting: true,
               },
               sortable: true,
               pageable: true,
               scrollable: false,
               columns: [
                   {
                       field: "StreetName",
                       title: "Vejnavn"
                   }, {
                       field: "StreetNumber",
                       title: "Husnummer"
                   }, {
                       field: "ZipCode",
                       title: "Postnummer"
                   }, {
                       field: "Town",
                       title: "By"

                   }, {
                       field: "Id",
                       template: "<a ng-click=editClick(${Id})>Redigér</a> | <a ng-click=deleteClick(${Id})>Slet</a>",
                       title: "Muligheder",
                   }
               ],
           };
       }

       $scope.updateAddressGrid = function () {
           $scope.gridContainer.addressGrid.dataSource.read();
       }

       $scope.editClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/EditAddressTemplate.html',
               controller: 'EditAddressController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function (editedAddress) {
               var result = AddressFormatter.fn(editedAddress);
               StandardAddress.patch({ id: id }, {
                   "StreetName": result.StreetName,
                   "StreetNumber": result.StreetNumber,
                   "ZipCode": result.ZipCode,
                   "Town": result.Town
               }, function () {
                   $scope.updateAddressGrid();
               });
           });
       }

       $scope.deleteClick = function(id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/ConfirmDeleteAddressTemplate.html',
               controller: 'DeleteAddressController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function () {
               StandardAddress.delete({ id: id }, function () {
                   $scope.updateAddressGrid();
               });
           });
       }

       $scope.addNewClick = function() {
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/AddNewAddressTemplate.html',
               controller: 'AddNewAddressController',
               backdrop: "static",
           });

           modalInstance.result.then(function (newAddress) {
               var result = AddressFormatter.fn(newAddress);
               StandardAddress.post({
                   "StreetName": result.StreetName,
                   "StreetNumber": result.StreetNumber,
                   "ZipCode": result.ZipCode,
                   "Town": result.Town,
                   "Latitude": "1",
                   "Longitude": "1"
               }, function () {
                   $scope.updateAddressGrid();
               });
           });
       }

       $scope.loadAddresses();
   }
]);