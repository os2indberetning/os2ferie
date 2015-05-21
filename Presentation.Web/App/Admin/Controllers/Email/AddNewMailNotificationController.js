angular.module("application").controller("AddNewMailNotificationController", [
    "$scope", "$modalInstance", "NotificationService", "StandardAddress", "AddressFormatter", "SmartAdresseSource",
    function ($scope, $modalInstance, NotificationService, StandardAddress, AddressFormatter, SmartAdresseSource) {


        $scope.repeatMonthly = "";

        $scope.dateOptions = {
            format: "dd/MM/yyyy"
        };

        $scope.notificationDate = new Date();

        $scope.confirmSave = function () {
            var error = false;

            $scope.repeatErrorMessage = "";
            if ($scope.repeatMonthly == "") {
                error = true;
                $scope.repeatErrorMessage = "* Du skal udfylde 'Gentag månedligt'.";
            }

            $scope.dateErrorMessage = "";
            if ($scope.notificationDate == undefined) {
                error = true;
                $scope.dateErrorMessage = "* Du skal vælge en gyldig adviseringsdato.";
            }

            var result = {};
            if ($scope.repeatMonthly == "true") {
                result.repeatMonthly = true;
            } else {
                result.repeatMonthly = false;
            }

            result.notificationDate = moment($scope.notificationDate).unix();

            if (!error) {
                $modalInstance.close(result);
                NotificationService.AutoFadeNotification("success", "", "Email-adviseringen blev oprettet.");
            }

        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Oprettelse af email adviseringen blev annulleret.");
        }
    }
]);