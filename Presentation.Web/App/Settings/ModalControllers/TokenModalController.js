angular.module("application").controller('TokenInstanceController', ["$scope", "NotificationService", "$modalInstance", "Token", "personId", "$modal", function ($scope, NotificationService, $modalInstance, Token, personId, $modal) {

    $scope.tokens = [];
    $scope.isCollapsed = true;
    $scope.newTokenDescription = "";

    Token.get({ id: personId }, function (data) {
        NotificationService.AutoFadeNotification("success", "Success", "Tokens blev fundet");
        $scope.tokens = data.value;
    }, function() {
        NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke hente tokens");
    });

    $scope.deleteToken = function (token) {
        var objIndex = $scope.tokens.indexOf(token);
        $scope.tokens.splice(objIndex, 1);

        Token.delete({ id: token.Id }, function (data) {
            NotificationService.AutoFadeNotification("success", "Success", "Token blev slettet");
        }, function () {
            $scope.tokens.push(token);
            NotificationService.AutoFadeNotification("danger", "Fejl", "Token blev ikke slettet");
        });
    }

    $scope.saveToken = function() {
        var newToken = new Token({
            PersonId: personId,
            Status: "Created",
            Description: $scope.newTokenDescription
        });

        newToken.$save(function (data) {
                $scope.tokens.push(data);
                NotificationService.AutoFadeNotification("success", "Success", "Ny token oprettet");
                $scope.newTokenDescription = "";
                $scope.isCollapsed = !$scope.isCollapsed;
        }, function () {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke oprette ny token");
        });
    }
    
    $scope.newToken = function () {
        $scope.isCollapsed = !$scope.isCollapsed;
    }

    $scope.closeTokenModal = function () {
        $modalInstance.close({
            
        });
    }

    $scope.openConfirmDeleteModal = function (token) {

        var modalInstance = $modal.open({
            templateUrl: '/App/Settings/confirmDeleteTokenModal.html',
            controller: 'confirmDeleteToken',
            backdrop: 'static',
            resolve: {
                token: function () {
                    return token;
                }
            }
        });

        modalInstance.result.then(function (tokenToDelete) {
            $scope.deleteToken(tokenToDelete);
        }, function () {
            
        });
    };
}]);

angular.module('application').controller('confirmDeleteToken', function ($scope, $modalInstance, token) {
    
    $scope.confirmDelete = function () {
        $modalInstance.close(token);
    };

    $scope.cancelDelete = function () {
        $modalInstance.dismiss('cancel');
    };
});