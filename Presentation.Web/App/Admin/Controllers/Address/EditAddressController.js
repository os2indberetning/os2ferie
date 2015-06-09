angular.module("application").controller("EditAddressController", [
    "$scope", "$modalInstance", "itemId", "NotificationService", "StandardAddress", "SmartAdresseSource", "$modal",
    function ($scope, $modalInstance, itemId, NotificationService, StandardAddress, SmartAdresseSource, $modal) {


        $scope.SmartAddress = SmartAdresseSource;

        StandardAddress.get({ id: itemId }).$promise.then(function (res) {
            var address = res.value[0];
            $scope.address = address.StreetName + " " + address.StreetNumber + ", " + address.ZipCode + " " + address.Town;
            $scope.description = address.Description;
            $scope.oldAddress = $scope.address;
            $scope.oldDescription = $scope.description;
        });



        $scope.confirmEdit = function () {
            /// <summary>
            /// Confirms edit of Standard Address
            /// </summary>
            var result = {};
            result.address = $scope.address;
            result.description = $scope.description;
            $modalInstance.close(result);
            NotificationService.AutoFadeNotification("success", "", "Adressen blev redigeret.");
        }

        $scope.cancel = function () {
            /// <summary>
            /// Cancels edit of Standard Address
            /// </summary>
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Redigering af adressen blev annulleret.");
        }

        $scope.showConfirmDiscardChangesModal = function () {
            /// <summary>
            /// Opens confirm discard changes modal.
            /// </summary>
            /// <param name="id"></param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Admin/HTML/Address/ConfirmDiscardChangesTemplate.html',
                controller: 'ConfirmDiscardChangesController',
                backdrop: "static",
            });

            modalInstance.result.then(function () {
                $scope.cancel();
            });

        }

        $scope.cancelClicked = function () {
            if ($scope.address != $scope.oldAddress || $scope.description != $scope.oldDescription) {
                $scope.showConfirmDiscardChangesModal();
            } else {
                $scope.cancel();
            }
        }
    }
]);