angular.module("application").controller('AddressEditModalInstanceController', ["$scope", "$modalInstance", "Address", "personId", "addressId", "NotificationService", "AddressFormatter", function ($scope, $modalInstance, Address, personId, addressId, NotificationService, AddressFormatter) {
    $scope.newAddress = "";
    $scope.oldAddressId = 0;
    $scope.oldAddress = "";
    $scope.addressDescription = "";

    $scope.loadAddressData= function() {
        if (addressId != undefined) {
            Address.get({ id: personId, query: "$filter=Id eq " + addressId }, function (data) {
                $scope.oldAddressId = data[0].Id;
                $scope.addressDescription = data[0].Description;
                $scope.oldAddress = data[0].StreetName + " " + data[0].StreetNumber + ", " + data[0].ZipCode + " " + data[0].Town;
            });
        }
    }

    $scope.loadAddressData();

    $scope.saveEditedAddress = function () {
        $scope.newAddress = $scope.oldAddress;

        var result = AddressFormatter.fn($scope.newAddress);

        if (addressId != undefined) {
            result.Id = $scope.oldAddressId;
            result.PersonId = personId;

            result.Description = $scope.addressDescription;

            var updatedAddress = new Address({
                PersonId: personId,
                StreetName: result.StreetName,
                StreetNumber: result.StreetNumber,
                ZipCode: parseInt(result.ZipCode),
                Town: result.Town,
                Description: $scope.addressDescription
            });

            updatedAddress.$patch({ id: result.Id }, function () {
                NotificationService.AutoFadeNotification("success", "Success", "Adresse opdateret");
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Adresse blev ikke opdateret");
            });
        } else {
            var newAddress = new Address({
                PersonId: personId,
                StreetName: result.StreetName,
                StreetNumber: result.StreetNumber,
                ZipCode: parseInt(result.ZipCode),
                Town: result.Town,
                Description: $scope.addressDescription,
                Latitude: "",
                Longitude: "",
                Type: "Standard"
            });

            newAddress.$post(function() {
                NotificationService.AutoFadeNotification("success", "Success", "Adresse oprettet");
            }, function() {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Adresse blev ikke oprettet");
            });
        }
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