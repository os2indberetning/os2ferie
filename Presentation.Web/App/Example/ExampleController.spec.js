/// <reference path="ExampleController.ts" /> 
'use strict';
describe('Controllers: ExampleController', function () {
    var $scope, ctrl;
    beforeEach(module('app.controllers'));
    beforeEach(inject(function ($rootScope, $controller) {
        $scope = $rootScope.$new();
        ctrl = $controller('HomeCtrl', { $scope: $scope });
    }));
    it('should set a page title', function () {
        expect($scope.$root.title).toBe('AngularJS SPA Template for Visual Studio');
    });
});
//# sourceMappingURL=ExampleController.spec.js.map