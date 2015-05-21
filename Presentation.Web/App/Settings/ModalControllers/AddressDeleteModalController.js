angular.module("application").controller('AddressDeleteModalInstanceController', [
    "$scope", "Address", "Point", "NotificationService", "$modalInstance", "addressId", "personId", "AddressFormatter", function ($scope, Address, Point, NotificationService, $modalInstance, addressId, personId, AddressFormatter) {

        $scope.confirmDelete = function () {
            Address.delete({ id: addressId }, function () {
                NotificationService.AutoFadeNotification("success", "", "Adresse slettet");
                $modalInstance.close('');
            });
        }

        $scope.cancelDelete = function () {
            $modalInstance.dismiss('');
        }

    }]);