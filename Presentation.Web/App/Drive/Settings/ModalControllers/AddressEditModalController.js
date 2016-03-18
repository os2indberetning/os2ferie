angular.module("app.drive").controller('AddressEditModalInstanceController', ["$scope", "$modalInstance", "Address", "personId", "addressId", "NotificationService", "AddressFormatter", "SmartAdresseSource", function ($scope, $modalInstance, Address, personId, addressId, NotificationService, AddressFormatter, SmartAdresseSource) {
    $scope.newAddress = "";
    $scope.oldAddressId = 0;
    $scope.oldAddress = "";
    $scope.addressDescription = "";

    $scope.loadAddressData= function() {
        if (addressId != undefined) {
            Address.get({ query: "$filter=Id eq " + addressId }, function (data) {
                $scope.oldAddressId = data.value[0].Id;
                $scope.addressDescription = data.value[0].Description;
                $scope.oldAddress = data.value[0].StreetName + " " + data.value[0].StreetNumber + ", " + data.value[0].ZipCode + " " + data.value[0].Town;
            });
        }
    }

    $scope.loadAddressData();

    $scope.addressNotFound = false;

    $scope.addressFieldOptions = {
            dataBound: function () {
                $scope.addressNotFound = this.dataSource._data.length == 0;
                $scope.$apply();
            }
        }

    $scope.saveEditedAddress = function () {

        if ($scope.addressNotFound) return;

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
                NotificationService.AutoFadeNotification("success", "", "Adresse opdateret");
                $modalInstance.close('');
            }, function () {
                NotificationService.AutoFadeNotification("danger", "", "Adresse blev ikke opdateret");
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
                NotificationService.AutoFadeNotification("success", "", "Adresse oprettet");
                $modalInstance.close('');
            }, function() {
                NotificationService.AutoFadeNotification("danger", "", "Adresse blev ikke oprettet");
            });
        }

    }

    $scope.SmartAddress = SmartAdresseSource;

    $scope.closeAddressEditModal = function () {
        $modalInstance.close({

        });
    };
}]);