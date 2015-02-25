describe('ApproveReports', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
    }));

    describe('AcceptController', function () {
        var controller;

        beforeEach(function () {
            modalInstance = {                    // Create a mock object using spies
                close: jasmine.createSpy('modalInstance.close'),
                dismiss: jasmine.createSpy('modalInstance.dismiss'),
                result: {
                    then: jasmine.createSpy('modalInstance.result.then')
                }
            };

           

            controller = $controller('AcceptController', { $scope: $scope, $modalInstance: modalInstance, itemId: 2});
        });

       it('YesClicked should call close with itemId', function () {
            $scope.yesClicked();
           expect(modalInstance.close).toHaveBeenCalledWith(2);
       });

        it('NoClicked should call dismiss with cancel', function () {
            $scope.noClicked();
            expect(modalInstance.dismiss).toHaveBeenCalledWith('cancel');
        });


    });
});