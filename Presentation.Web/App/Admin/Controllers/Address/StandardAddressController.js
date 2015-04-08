angular.module("application").controller("StandardAddressController", [
   "$scope", "$modal", "StandardAddress", "AddressFormatter", function ($scope, $modal, StandardAddress, AddressFormatter) {

        $scope.gridContainer = {};

       $scope.loadAddresses = function () {
           $scope.addresses = {
               dataSource: {
                   type: "odata",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/Addresses/Service.GetStandard",
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
                   serverPaging: false,
                   serverSorting: true,
               },
               sortable: true,
               pageable: {
                   messages: {
                       display: "{0} - {1} af {2} adresser", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                       empty: "Ingen adresser at vise",
                       page: "Side",
                       of: "af {0}", //{0} is total amount of pages
                       itemsPerPage: "adresser pr. side",
                       first: "Gå til første side",
                       previous: "Gå til forrige side",
                       next: "Gå til næste side",
                       last: "Gå til sidste side",
                       refresh: "Genopfrisk"
                   },
                   pageSizes: [5, 10, 20, 30, 40, 50]
               },
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
                       field: "Description",
                       title: "Beskrivelse"
                   },
                   {
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

       $scope.gridContainer.gridPageSize = 20;

       $scope.editClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/HTML/Address/EditAddressTemplate.html',
               controller: 'EditAddressController',
               backdrop: "static",
               resolve: {
                   itemId: function () {
                       return id;
                   }
               }
           });

           modalInstance.result.then(function (res) {
               var result = AddressFormatter.fn(res.address);
               StandardAddress.patch({ id: id }, {
                   "StreetName": result.StreetName,
                   "StreetNumber": result.StreetNumber,
                   "ZipCode": result.ZipCode,
                   "Town": result.Town,
                   "Description": res.description
               }, function () {
                   $scope.updateAddressGrid();
               });
           });
       }

       $scope.pageSizeChanged = function () {
           $scope.gridContainer.addressGrid.dataSource.pageSize(Number($scope.gridContainer.gridPageSize));
       }

       $scope.deleteClick = function (id) {
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/HTML/Address/ConfirmDeleteAddressTemplate.html',
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

       $scope.addNewClick = function () {
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/HTML/Address/AddNewAddressTemplate.html',
               controller: 'AddNewAddressController',
               backdrop: "static",
           });

           modalInstance.result.then(function (res) {
               var result = AddressFormatter.fn(res.address);
               StandardAddress.post({
                   "StreetName": result.StreetName,
                   "StreetNumber": result.StreetNumber,
                   "ZipCode": result.ZipCode,
                   "Town": result.Town,
                   "Description": res.description,
                   "Latitude": "0",
                   "Longitude": "0"
               }, function () {
                   $scope.updateAddressGrid();
               });
           });
       }

       $scope.loadAddresses();
   }
]);