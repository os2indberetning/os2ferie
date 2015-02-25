angular.module("application").controller("StandardAddressController", [
   "$scope", "$modal", "StandardAddress", function ($scope, $modal, StandardAddress) {

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

           modalInstance.result.then(function () {
               // onSuccess
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

       $scope.loadAddresses();
   }
]);