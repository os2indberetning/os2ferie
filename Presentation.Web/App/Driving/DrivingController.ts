 /// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />

module Driving {
    'use strict';
    export interface Scope extends ng.IScope {
        state: ng.ui.IState;
        date : any;
    }

    export class Controller {

        constructor(private $scope: Scope) {

            $scope.date = {
                start: "month",
                value: new Date()
            }

        }
    }
}

Application.AngularApp.Module.controller("DrivingController", Driving.Controller);