angular.module("application").controller('TokenInstanceController', ["$scope", "NotificationService", "$modalInstance", "Token", "items", "personId", function ($scope, NotificationService, $modalInstance, Token, items, personId) {

    $scope.tokens = items;    

    $scope.deleteToken = function (token) {
        var objIndex = $scope.licenseplates.indexOf(token);
        $scope.tokens.splice(objIndex, 1);

        Token.delete({ id: token.Id }, function(data) {
            NotificationService.AutoFadeNotification("success", "Success", "Token blev slettet");
        }, function() {
            $scope.tokens.push(token);
            NotificationService.AutoFadeNotification("danger", "Fejl", "Token blev ikke slettet");
        });

    }

    $scope.saveNewToken = function () {
        var newToken = new Token({
            PersonId: personId
        });

        newToken.$save(function (data) {
            $scope.tokens.push(data.value[0]);
            NotificationService.AutoFadeNotification("success", "Success", "Ny token oprettet");
        }, function () {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke oprette ny token");
        });
    }

    $scope.closeTokenModal = function () {
        $modalInstance.close({
            tokens: $scope.tokens
        });
    }

    //$scope.closeTokenModal = function () {
    //    $modalInstance.dismiss('Luk');
    //};
}]);