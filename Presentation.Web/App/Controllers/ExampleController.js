/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
var Application;
(function (Application) {
    "use strict";
    var AppCtrl = (function () {
        function AppCtrl($scope) {
            this.$scope = $scope;
            $scope.greeting = "asdf";
            $scope.changeName = function (name) {
                $scope.greeting = "Hello " + name + " !";
            };
        }
        AppCtrl.$inject = [
            "$scope"
        ];
        return AppCtrl;
    })();
    Application.AppCtrl = AppCtrl;
    Application.AngularApp.Module.controller("appCtrl", AppCtrl);
})(Application || (Application = {}));
//# sourceMappingURL=ExampleController.js.map