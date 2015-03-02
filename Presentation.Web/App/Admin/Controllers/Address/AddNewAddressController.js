angular.module("application").controller("AddNewAddressController", [
    "$scope", "$modalInstance", "NotificationService", "StandardAddress", "AddressFormatter", "SmartAdresseSource", function ($scope, $modalInstance, NotificationService, AddressFormatter, SmartAdresseSource) {

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

      

        $scope.confirmSave = function () {
            var result = {};
            result.address = $scope.address;
            result.description = $scope.description;
            $modalInstance.close(result);
            NotificationService.AutoFadeNotification("success", "Opret", "Adressen blev oprettet.");
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "Opret", "Oprettelse af adressen blev annulleret.");
        }
    }
]);