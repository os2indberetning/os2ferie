angular.module("application").controller("DeleteAddressController", [
    "$scope", "$modalInstance", "itemId", "NotificationService", "StandardAddress",
    function ($scope, $modalInstance, itemId, NotificationService, StandardAddress) {

        StandardAddress.get({ id: itemId }).$promise.then(function (res) {
            var address = res.value[0];
            $scope.addressString = address.StreetName + " " + address.StreetNumber + ", " + address.ZipCode + " " + address.Town + ".";
        });

      
        $scope.confirmDelete = function () {
            $modalInstance.close($scope.itemId);
            NotificationService.AutoFadeNotification("success", "Slet", "Adressen blev slettet.");
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "Slet", "Sletning af adressen blev annulleret.");
        }
    }
]);