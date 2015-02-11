angular.module("application").controller("SettingController", [
    "$scope", "Person", "$http", function ($scope, Person, $http) {
        $scope.isCollapsed = true;
        $scope.mailAdvice = 'No';
        $scope.licenseplates = [];

        $scope.setHomeWorkOverride = function () {
            $http({ method: 'PATCH', url: "odata/Person(1)", data: { workDistanceOverride: 42 } })
                .success();
        };

        //$scope.GetLicensePlates = function (personId) {
        //    return $http({
        //        method: 'GET',
        //        url: 'odata/LicensePlates(' + personId + ')'
        //    }).success();
        //}

        $scope.GetPerson = Person.get({ id: 1 }, function(data) {
                console.log(data);
            },
        function() {
            
        });

        
    }
]);

angular.module("application").controller('TokenController', ["$scope", "$modal", function ($scope, $modal) {

    $scope.items = ['Nr. 1: 123456', 'Nr. 1: 234567', 'Nr. 1: 345678'];

    $scope.openTokenModal = function (size) {

        var modalInstance = $modal.open({
            templateUrl: '/App/Settings/tokenModal.html',
            controller: 'TokenInstanceController',
            size: size,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        });
    };
}]);

angular.module("application").controller('TokenInstanceController', ["$scope", "$modalInstance", "items", function ($scope, $modalInstance, items) {

    $scope.items = items;
    $scope.selected = {
        item: $scope.items[0]
    };

    $scope.closeTokenModal = function () {
        $modalInstance.dismiss('Luk');
    };
}]);