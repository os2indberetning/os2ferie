/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />

module Application {
    "use strict";
    export interface IAppCtrlScope extends ng.IScope {
        greeting: string;
        changeName(name : string): void;
    }
    export class AppCtrl {
        public static $inject = [
            "$scope"
        ];
        constructor(private $scope: IAppCtrlScope) {
            $scope.greeting = "asdf";
            $scope.changeName = (name: string) => {
                $scope.greeting = "Hello " + name + " !";
            };
        }
    }

    Application.AngularApp.Module.controller("appCtrl", AppCtrl);
} 