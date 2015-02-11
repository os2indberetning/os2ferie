/// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />
var Navbar;
(function (Navbar) {
    'use strict';
    var Controller = (function () {
        function Controller($scope) {
            this.$scope = $scope;
        }
        return Controller;
    })();
    Navbar.Controller = Controller;
})(Navbar || (Navbar = {}));
Application.AngularApp.Module.controller("NavbarController", Navbar.Controller);
//# sourceMappingURL=NavbarController.js.map