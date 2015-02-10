 /// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />

module Driving {
    'use strict';
    interface Scope extends ng.IScope {
        state: ng.ui.IState;
        date : any;
    }

    export class Controller {

        constructor(private $scope: Scope, private $personResource : IPersonResource) {


            console.log($personResource.get())

            $scope.date = {
                start: "month",
                value: new Date()
            }

        }
    }
}

Application.AngularApp.Module.controller("DrivingController", Driving.Controller);