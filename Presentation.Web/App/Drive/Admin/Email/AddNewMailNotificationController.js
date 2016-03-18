angular.module("app.drive").controller("AddNewMailNotificationController", [
    "$scope", "$modalInstance", "NotificationService", "StandardAddress", "AddressFormatter", "SmartAdresseSource",
    function ($scope, $modalInstance, NotificationService, StandardAddress, AddressFormatter, SmartAdresseSource) {


        $scope.repeatMonthly = "";

        $scope.dateOptions = {
            format: "dd/MM/yyyy"
        };

        $scope.notificationDate = new Date();

        $scope.confirmSave = function () {
            /// <summary>
            /// Saves new MailNotification if fields are properly filled.
            /// </summary>
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

            $scope.payDateErrorMessage = "";
            if ($scope.payRoleDate == undefined) {
                error = true;
                $scope.payDateErrorMessage = "* Du skal vælge en gyldig lønkørselsdato.";
            }

            var result = {};
            if ($scope.repeatMonthly == "true") {
                result.repeatMonthly = true;
            } else {
                result.repeatMonthly = false;
            }

            result.notificationDate = moment($scope.notificationDate).unix();
            result.payDate = moment($scope.payRoleDate).unix();
            if (!error) {
                $modalInstance.close(result);
                NotificationService.AutoFadeNotification("success", "", "Email-adviseringen blev oprettet.");
            }

        }

        $scope.cancel = function () {
            /// <summary>
            /// Cancels creation of new MailNotification.
            /// </summary>
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Oprettelse af email adviseringen blev annulleret.");
        }
    }
]);