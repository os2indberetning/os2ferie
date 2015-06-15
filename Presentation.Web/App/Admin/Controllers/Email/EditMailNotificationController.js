angular.module("application").controller("EditMailNotificationController", [
    "$scope", "$modalInstance", "itemId", "NotificationService", "EmailNotification",
    function ($scope, $modalInstance, itemId, NotificationService, EmailNotification) {


     

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
            var date = moment.unix(res.DateTimestamp);
            var payDate = moment.unix(res.PayRoleTimestamp);
            $scope.notificationDate = date._d;
            $scope.payRoleDate = payDate._d;
        });

        $scope.confirmSave = function () {
            /// <summary>
            /// Confirms edit of MailNotification.
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
            result.repeatMonthly = $scope.repeatMonthly.bool;

            result.notificationDate = moment($scope.notificationDate).unix();

            result.payDate = moment($scope.payRoleDate).unix();

            if (!error) {
                $modalInstance.close(result);
                NotificationService.AutoFadeNotification("success", "", "Email-adviseringen blev redigeret.");
            }
            
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
            NotificationService.AutoFadeNotification("warning", "", "Oprettelse af email adviseringen blev annulleret.");
        }
    }
]);