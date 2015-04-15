angular.module("application").controller("EditAddressController", [
    "$scope", "$modalInstance", "itemId", "NotificationService", "StandardAddress", "SmartAdresseSource",
    function ($scope, $modalInstance, itemId, NotificationService, StandardAddress, SmartAdresseSource) {


        $scope.SmartAddress = SmartAdresseSource;

       StandardAddress.get({ id: itemId }).$promise.then(function(res) {
            var address = res.value[0];
            $scope.address = address.StreetName + " " + address.StreetNumber + ", " + address.ZipCode + " " + address.Town;
            $scope.description = address.Description;
        });



        $scope.confirmEdit = function () {
            var result = {};
            result.address = $scope.address;
            result.description = $scope.description;
            $modalInstance.close(result);
            NotificationService.AutoFadeNotification("success", "", "Adressen blev redigeret.");
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Redigering af adressen blev annulleret.");
        }
    }
]);