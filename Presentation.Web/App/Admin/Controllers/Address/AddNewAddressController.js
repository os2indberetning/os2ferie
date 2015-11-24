angular.module("application").controller("AddNewAddressController", [
    "$scope", "$modalInstance", "NotificationService", "StandardAddress", "AddressFormatter", "SmartAdresseSource", "$modal",
    function ($scope, $modalInstance, NotificationService, StandardAddress, AddressFormatter, SmartAdresseSource, $modal) {

        $scope.SmartAddress = SmartAdresseSource;

        $scope.confirmSave = function () {
            /// <summary>
            /// Confirms creation of new Standard Address
            /// </summary>
            var result = {};
            result.address = $scope.Address.Name;
            result.description = $scope.description;
            $modalInstance.close(result);
            NotificationService.AutoFadeNotification("success", "", "Adressen blev oprettet.");
        }

        $scope.cancel = function () {
            /// <summary>
            /// Cancels creation of new Standard Address
            /// </summary>
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Oprettelse af adressen blev annulleret.");
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
            if ($scope.description == undefined) {
                if ($scope.Address == undefined || $scope.Address.Name == undefined) {
                    $scope.cancel();
                } else {
                    $scope.showConfirmDiscardChangesModal();
                }
            } else {
                $scope.showConfirmDiscardChangesModal();
            }
        }
    }
]);