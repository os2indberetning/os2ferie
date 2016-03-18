angular.module("app.drive").controller("DeleteAddressController", [
    "$scope", "$modalInstance", "itemId", "NotificationService", "StandardAddress",
    function ($scope, $modalInstance, itemId, NotificationService, StandardAddress) {

        StandardAddress.get({ id: itemId }).$promise.then(function (res) {
            var address = res.value[0];
            $scope.addressString = address.StreetName + " " + address.StreetNumber + ", " + address.ZipCode + " " + address.Town + ".";
        });

      
        $scope.confirmDelete = function () {
            /// <summary>
            /// Confirms deletion of Standard Address
            /// </summary>
            $modalInstance.close($scope.itemId);
            NotificationService.AutoFadeNotification("success", "", "Adressen blev slettet.");
        }

        $scope.cancel = function () {
            /// <summary>
            /// Cancels deletion of Standard Address
            /// </summary>
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Sletning af adressen blev annulleret.");
        }
    }
]);