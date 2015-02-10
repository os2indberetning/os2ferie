 /// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />

module Driving {
    'use strict';
    interface Scope extends ng.IScope {
        state: ng.ui.IState;
        date: any;        
    }

    export class Controller {           
        private personResource: ServiceModels.IPersonResource;
        constructor(private $scope: Scope, private $personResource : ServiceModels.IPersonResource) {
            this.personResource = $personResource;

            var test = this.personResource.get({ id: 1 }, function () {
                console.log(test);
            });

            

            $scope.date = {
                start: "month",
                value: new Date()
            }

        }
    }
}

Application.AngularApp.Module.controller("DrivingController", Driving.Controller);