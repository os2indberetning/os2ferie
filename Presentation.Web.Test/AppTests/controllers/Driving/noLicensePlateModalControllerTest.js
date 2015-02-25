describe('Driving', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
    }));

    describe('noLicensePlateModalController', function () {
        var controller;

        beforeEach(function () {
            modalInstance = {                    // Create a mock object using spies
                close: jasmine.createSpy('modalInstance.close'),
                dismiss: jasmine.createSpy('modalInstance.dismiss'),
                result: {
                    then: jasmine.createSpy('modalInstance.result.then')
                }
            };



            controller = $controller('noLicensePlateModalController', { $scope: $scope, $modalInstance: modalInstance, itemId: 2 });
        });

        it('YesClicked should call close with itemId', function () {
            $scope.ok();
            expect(modalInstance.close).toHaveBeenCalled();
        });

        it('NoClicked should call dismiss with cancel', function () {
            $scope.cancel();
            expect(modalInstance.dismiss).toHaveBeenCalledWith('cancel');
        });
    

    });
});