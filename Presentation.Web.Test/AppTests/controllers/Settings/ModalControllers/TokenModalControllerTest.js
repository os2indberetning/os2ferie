describe('TokenInstanceController', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope, tokenMock;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
    }));

    describe('TokenInstanceController', function () {
        var controller;

        beforeEach(function () {
            modalInstance = {                    // Create a mock object using spies
                close: jasmine.createSpy('modalInstance.close'),
                dismiss: jasmine.createSpy('modalInstance.dismiss'),
                result: {
                    then: jasmine.createSpy('modalInstance.result.then')
                }
            };
            tokenMock = {                    // Create a mock object using spies
                delete: jasmine.createSpy('tokenMock.delete'),
                get: jasmine.createSpy('tokenMock.get'),
            };

 

        

            controller = $controller('TokenInstanceController', { $scope: $scope, $modalInstance: modalInstance, personId : 1, Token : tokenMock});
        });

        it('Scope.NewToken should negate scope.isCollapsed', function() {
            var preRes = $scope.isCollapsed;

            $scope.newToken();

            expect($scope.isCollapsed).toBe(!preRes);
        });

        it('scope.closeTokenModal should call modalInstance.close', function () {
            $scope.closeTokenModal();
            expect(modalInstance.close).toHaveBeenCalled();
        });

        it('scope.deleteToken should call Token.delete', function() {
            $scope.deleteToken({ Id: 2 });
            expect(tokenMock.delete).toHaveBeenCalled();
        });


    });
});

