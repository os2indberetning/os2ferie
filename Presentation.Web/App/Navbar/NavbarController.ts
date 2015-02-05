/// <reference path="../../scripts/typings/kendo/kendo.all.d.ts" />

module Navbar {
    'use strict';
    export interface Scope extends ng.IScope {
        $state: ng.ui.IState
    }

    export class Controller {

        constructor(private $scope: Scope) {

            

        }
    }
}

Application.AngularApp.Module.controller("NavbarController", Navbar.Controller);