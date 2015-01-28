/// <reference path="../Scripts/typings/angularjs/angular.d.ts" />
var Application;
(function (Application) {
    "use strict";
    var AngularApp = (function () {
        function AngularApp() {
        }
        AngularApp.Module = angular.module("app", []);
        return AngularApp;
    })();
    Application.AngularApp = AngularApp;
})(Application || (Application = {}));
//# sourceMappingURL=App.js.map