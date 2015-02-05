/// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />
var Driving;
(function (Driving) {
    'use strict';
    var Controller = (function () {
        function Controller($scope) {
            this.$scope = $scope;
            $scope.date = {
                start: "month",
                value: new Date()
            };
        }
        return Controller;
    })();
    Driving.Controller = Controller;
})(Driving || (Driving = {}));
Application.AngularApp.Module.controller("DrivingController", Driving.Controller);
//# sourceMappingURL=DrivingController.js.map