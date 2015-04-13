angular.module("application").controller("AddNewAddressController", [
    "$scope", "$modalInstance", "NotificationService", "StandardAddress", "AddressFormatter", "SmartAdresseSource", function ($scope, $modalInstance, NotificationService, AddressFormatter, SmartAdresseSource) {

        $scope.SmartAddress = {
            type: "json",
            minLength: 3,
            serverFiltering: true,
            crossDomain: true,
            transport: {
                read: {
                    url: function(item) {
                        var req = 'http://dawa.aws.dk/adgangsadresser/autocomplete?q=' + item.filter.filters[0].value;
                        return req;
                    },
                    dataType: "jsonp",
                    data: {

                    }
                }
            },
            schema: {
                data: function(data) {
                    return data; // <-- The result is just the data, it doesn't need to be unpacked.
                }
            },
        };
         

      

        $scope.confirmSave = function () {
            var result = {};
            result.address = $scope.Address.Name;
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