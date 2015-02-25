describe('RouteEditModalInstanceController', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope, routeMock;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
    }));

    describe('RouteEditModalInstanceController', function () {
        var controller;

        beforeEach(function () {
            modalInstance = {                    // Create a mock object using spies
                close: jasmine.createSpy('modalInstance.close'),
                dismiss: jasmine.createSpy('modalInstance.dismiss'),
                result: {
                    then: jasmine.createSpy('modalInstance.result.then')
                }
            };
            routeMock = {                    // Create a mock object using spies
                get: jasmine.createSpy('routeMock.get'),
                patch: jasmine.createSpy('routeMock.patch'),
                post: jasmine.createSpy('routeMock.post')
            };






            controller = $controller('RouteEditModalInstanceController', { $scope: $scope, $modalInstance: modalInstance, routeId: 2, personId: 1, Route: routeMock });
        });


        it('scope.closeRouteEditModal should call modalInstance.close', function () {
            $scope.closeRouteEditModal();
            expect(modalInstance.close).toHaveBeenCalled();
        });

        it('scope.getRouteInformation should call Route.get when routeId is defined', function () {
            $scope.getRouteInformation();
            expect(routeMock.get).toHaveBeenCalled();
        });

        it('scope.addNewViaPoint should increment the size of scope.viapoints', function() {
            var preRes = $scope.viaPoints.length;
            $scope.addNewViaPoint();
            expect($scope.viaPoints.length).toBe(preRes + 1);
        });


    });
});

