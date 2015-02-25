describe('confirmDeleteToken', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope, tokenMock;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
    }));

    describe('confirmDeleteToken', function () {
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
                del: jasmine.createSpy('tokenMock.delete'),
            };





            controller = $controller('confirmDeleteToken', { $scope: $scope, $modalInstance: modalInstance, token: tokenMock });
        });


        it('confirmDelete should call modalInstance.close', function () {
            $scope.confirmDelete();
            expect(modalInstance.close).toHaveBeenCalled();
        });

        it('cancelDelete should call modalInstance.dismiss with cancel', function () {
            $scope.cancelDelete();
            expect(modalInstance.dismiss).toHaveBeenCalledWith('cancel');
        });








    });
});

