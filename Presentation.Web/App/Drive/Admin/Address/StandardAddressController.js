angular.module("app.drive").controller("StandardAddressController", [
   "$scope", "$modal", "StandardAddress", "AddressFormatter", function ($scope, $modal, StandardAddress, AddressFormatter) {

        $scope.gridContainer = {};

        $scope.loadAddresses = function () {
            /// <summary>
            /// Loads existing standard addresses from backend.
            /// </summary>
           $scope.addresses = {
               dataSource: {
                   type: "odata-v4",
                   transport: {
                       read: {
                           beforeSend: function (req) {
                               req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                           },
                           url: "/odata/Addresses/Service.GetStandard",
                           dataType: "json",
                           cache: false
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
                   pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
               },
               scrollable: false,
               columns: [
                   {
                       field: "StreetName",
                       title: "Vejnavn"
                   },
                   {
                       field: "StreetNumber",
                       title: "Husnummer"
                   },
                   {
                       field: "ZipCode",
                       title: "Postnummer"
                   },
                   {
                       field: "Town",
                       title: "By"

                   },
                   {
                       field: "Description",
                       title: "Beskrivelse"
                   },
                   {
                       field: "Id",
                       template: "<a ng-click=editClick(${Id})>Redigér</a> | <a ng-click=deleteClick(${Id})>Slet</a>",
                       title: "Muligheder",
                   }
               ]
           };
       }

        $scope.updateAddressGrid = function () {
            /// <summary>
            /// Refreshes standard address grid.
            /// </summary>
           $scope.gridContainer.addressGrid.dataSource.read();
       }

        $scope.editClick = function (id) {
            /// <summary>
            /// Opens standard address edit modal.
            /// </summary>
            /// <param name="id">Id of address to be edited.</param>
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/Address/EditAddressTemplate.html',
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

        $scope.deleteClick = function (id) {
            /// <summary>
            /// Opens delete StandardAddress modal
            /// </summary>
            /// <param name="id">Id of address to be deleted.</param>
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/Address/ConfirmDeleteAddressTemplate.html',
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
            /// <summary>
            /// Opens add new Standard Address modal
            /// </summary>
           var modalInstance = $modal.open({
               templateUrl: '/App/Admin/Address/AddNewAddressTemplate.html',
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