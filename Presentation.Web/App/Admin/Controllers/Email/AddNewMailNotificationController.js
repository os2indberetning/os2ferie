angular.module("application").controller("AddNewMailNotificationController", [
    "$scope", "$modalInstance", "NotificationService", "StandardAddress", "AddressFormatter", "SmartAdresseSource", function ($scope, $modalInstance, NotificationService, AddressFormatter, SmartAdresseSource) {

        $scope.repeatMonthly = "";

        $scope.dateOptions = {
            format: "dd/MM/yyyy"
        }

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
                $scope.dateErrorMessage = "* Du skal vælge en adviseringsdato.";
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
                NotificationService.AutoFadeNotification("success", "Opret", "Email-adviseringen blev oprettet.");
            }
            
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "Opret", "Oprettelse af email adviseringen blev annulleret.");
        }
    }
]);