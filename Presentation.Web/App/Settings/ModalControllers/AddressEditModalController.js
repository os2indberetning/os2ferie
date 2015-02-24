﻿angular.module("application").controller('AddressEditModalController', ["$scope", "$modalInstance", "Address", "personId", "addressId", "NotificationService", "AddressFormatter", function ($scope, $modalInstance, Address, personId, addressId, NotificationService, AddressFormatter) {
    $scope.newAddress = "";
    $scope.oldAddressId = 0;
    $scope.oldAddress = "";
    $scope.addressDescription = "";

    Address.get({ id: personId, query: "$filter=Id eq " + addressId }, function (data) {
        $scope.oldAddressId = data.value[0].Id;
        $scope.addressDescription = data.value[0].Description;
        $scope.oldAddress = data.value[0].StreetName + " " + data.value[0].StreetNumber + ", " + data.value[0].ZipCode + " " + data.value[0].Town;
    });

    $scope.saveEditedAddress = function () {
        $scope.newAddress = $scope.oldAddress;

        var result = AddressFormatter.fn($scope.newAddress);

        result.Id = $scope.oldAddressId;
        result.PersonId = personId;

        result.Description = $scope.addressDescription;

        var address = new Address({
            Id: result.Id,
            PersonId: result.PersonId,
            StreetName: result.StreetName,
            StreetNumber: result.StreetNumber,
            ZipCode: result.ZipCode,
            Town: result.Town,
            Description: result.Description
        });

        address.$patch({ id: result.Id }, function () {
            NotificationService.AutoFadeNotification("success", "Success", "Adresse opdateret");
        }, function() {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Adresse blev ikke opdateret");
        });
    }

    $scope.SmartAddress = {
        type: "json",
        minLength: 3,
        serverFiltering: true,
        crossDomain: true,
        transport: {
            read: {
                url: function (item) {
                    return 'https://smartadresse.dk/service/locations/3/detect/json/' + item.filter.filters[0].value + '%200';
                },
                dataType: "jsonp",
                data: {
                    apikey: 'FCF3FC50-C9F6-4D89-9D7E-6E3706C1A0BD',
                    limit: 15,                   // REST limit
                    crs: 'EPSG:25832',           // REST projection
                    nogeo: 'true',                 // REST nogeo
                    noadrspec: 'true'             // REST noadrspec
                }
            }
        },
        schema: {
            data: function (data) {
                return data.data; // <-- The result is just the data, it doesn't need to be unpacked.
            }
        },
    }

    $scope.closeAddressEditModal = function () {
        $modalInstance.close({

        });
    };
}]);