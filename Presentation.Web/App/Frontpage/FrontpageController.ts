module Frontpage {
    'use strict';
    export interface Scope extends ng.IScope {

    }

    export class Controller {
        private httpService: any;

        constructor($scope: Scope, $http: any) {
        }

    }
}

Application.AngularApp.Module.controller("FrontpageController", Frontpage.Controller);