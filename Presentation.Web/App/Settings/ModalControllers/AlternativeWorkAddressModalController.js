angular.module("application").controller('AlternativeWorkAddressModalController', ["$scope", "$modalInstance", "SmartAdresseSource", "$rootScope", function ($scope, $modalInstance, SmartAdresseSource, $rootScope) {

    $scope.employments = $rootScope.CurrentUser.Employments;
    $scope.Number = Number;
    $scope.toString = toString;
    $scope.replace = String.replace;

    $scope.SmartAddress = SmartAdresseSource;

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}]);