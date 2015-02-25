describe('ApproveReports', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
    }));

    describe('RejectController', function () {
        var controller;

        beforeEach(function () {
            modalInstance = {                    // Create a mock object using spies
                close: jasmine.createSpy('modalInstance.close'),
                dismiss: jasmine.createSpy('modalInstance.dismiss'),
                result: {
                    then: jasmine.createSpy('modalInstance.result.then')
                }
            };

           

            controller = $controller('RejectController', { $scope: $scope, $modalInstance: modalInstance, itemId: 2});
        });

        it('Errormessage should be undefined initially', function () {
            expect($scope.errorMessage).toBe(undefined);
        });

        it('Errormessage should be set when no comment is given and yes is clicked', function () {
            $scope.yesClicked();
            expect($scope.errorMessage).toBe("* Du skal angive en kommentar.");
        });

        it('YesClicked should return the id and comment when comment is given', function () {
            $scope.result = {};
            $scope.comment = "TestComment";

            var expectedResult = { Id: 2, Comment: "TestComment" };

            $scope.yesClicked();
            expect(modalInstance.close).toHaveBeenCalledWith(expectedResult);
        });

        it('NoClicked should call dismiss with cancel', function () {
            $scope.noClicked();
            expect(modalInstance.dismiss).toHaveBeenCalledWith('cancel');
        });


    });
});