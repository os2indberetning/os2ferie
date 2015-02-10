 /// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />
module Driving {
    'use strict';
    interface Scope extends ng.IScope {
        state: ng.ui.IState;
        date: any;        
    }

    export class Controller {           
        private Person: ServiceModels.IPersonResource;

        constructor($scope: Scope, Person: ServiceModels.IPersonResource) {
            this.Person = Person;

            var test = this.Person.get({id:1}, function () {
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