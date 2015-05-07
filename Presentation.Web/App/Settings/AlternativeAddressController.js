angular.module("application").controller('AlternativeAddressController', ["$scope", "SmartAdresseSource", "$rootScope", function ($scope, SmartAdresseSource, $rootScope) {

    $scope.employments = $rootScope.CurrentUser.Employments;
    $scope.Number = Number;
    $scope.toString = toString;
    $scope.replace = String.replace;





  



    $scope.SmartAddress = SmartAdresseSource;

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}]);