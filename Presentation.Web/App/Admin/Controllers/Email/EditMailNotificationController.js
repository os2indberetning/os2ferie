angular.module("application").controller("EditMailNotificationController", [
    "$scope", "$modalInstance", "itemId", "NotificationService", "EmailNotification", function ($scope, $modalInstance, itemId, NotificationService, EmailNotification) {

        $scope.repeatMonthly = "";

        $scope.dateOptions = {
            format: "dd/MM/yyyy"
        };

        $scope.repeatItems = [{ value: "Ja", bool : true }, { value: "Nej", bool : false }];

        EmailNotification.get({ id: itemId }).$promise.then(function (res) {
            $scope.repeatMonthly = $scope.repeatItems[1];
            if (res.Repeat) {
                $scope.repeatMonthly = $scope.repeatItems[0];
            }
            var t = moment.unix(res.DateTimestamp);
            $scope.notificationDate = t._d;

        });

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
            result.repeatMonthly = $scope.repeatMonthly.bool;

            result.notificationDate = moment($scope.notificationDate).unix();

            if (!error) {
                $modalInstance.close(result);
                NotificationService.AutoFadeNotification("success", "Redigér", "Email-adviseringen blev redigeret.");
            }
            
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "Redigér", "Oprettelse af email adviseringen blev annulleret.");
        }
    }
]);