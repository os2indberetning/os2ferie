describe('ApproveReports', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, bankAccountMock, $q, $scope;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $q = _$q_;
        $scope = _$rootScope_.$new();
    }));

    describe('AcceptWithAccountController', function () {
        var controller;

        beforeEach(function () {
            modalInstance = {                    // Create a mock object using spies
                close: jasmine.createSpy('modalInstance.close'),
                dismiss: jasmine.createSpy('modalInstance.dismiss'),
                result: {
                    then: jasmine.createSpy('modalInstance.result.then')
                }
            };

            bankAccountMock = {
                get: function() {
                    queryDeferred = $q.defer();
                    queryDeferred.resolve({ Id: 2, Number: 123, Description: "Desc" });
                   
                    return { $promise: queryDeferred.promise };
                }
            }


            controller = $controller('AcceptWithAccountController', { $scope: $scope, $modalInstance : modalInstance, itemId : 2, BankAccount : bankAccountMock});
        });

        it('Errormessage should be undefined initially', function() {
            expect($scope.errorMessage).toBe(undefined);
        });

        it('Errormessage should be set when no account is selected and yes is clicked', function () {
            $scope.yesClicked();
            expect($scope.errorMessage).toBe("* Du skal vælge en konto");
        });

        it('YesClicked should return the selected account', function () {
            $scope.result = {};
            $scope.selectedAccount = { Id: 2, Number: "123456" };

            var expectedResult = { Id: 2, AccountNumber: "123456" };

            $scope.yesClicked();
            expect(modalInstance.close).toHaveBeenCalledWith(expectedResult);
        });

        it('NoClicked should call dismiss with cancel', function() {
            $scope.noClicked();
            expect(modalInstance.dismiss).toHaveBeenCalledWith('cancel');
        });

        
    });
});