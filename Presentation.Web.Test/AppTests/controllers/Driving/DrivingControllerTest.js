describe('Driving', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope, personEmploymentMock, licensePlateMock;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
    }));

    describe('DrivingController', function () {
        var controller;

        beforeEach(function () {
            modalInstance = {                    // Create a mock object using spies
                close: jasmine.createSpy('modalInstance.close'),
                dismiss: jasmine.createSpy('modalInstance.dismiss'),
                result: {
                    then: jasmine.createSpy('modalInstance.result.then')
                }
            };

            personEmploymentMock = {                    // Create a mock object using spies
                get: jasmine.createSpy('personEmploymentMock.get'),
            };

            licensePlateMock = {                    // Create a mock object using spies
                get: jasmine.createSpy('licensePlateMock.get'),
                del: jasmine.createSpy('licensePlateMock.delete')
            };



            controller = $controller('DrivingController', { $scope: $scope, $modalInstance: modalInstance, PersonEmployments: personEmploymentMock, LicensePlate : licensePlateMock});
        });




        it('Scope.DriveReport should be defined initially', function() {
            expect($scope.DriveReport).toBeDefined();
        });

        it('DriveReport.Addresses.Length should increase by one when calling AddViaPoint', function() {
            var preLength = $scope.DriveReport.Addresses.length;
            $scope.AddViapoint();
            expect($scope.DriveReport.Addresses.length).toBe(preLength + 1);
        });

        it('PersonEmployments.get should get called', function() {
            expect(personEmploymentMock.get).toHaveBeenCalled();
        });

        it('Scope.DriveReport.Addresses should be defined', function () {
            expect($scope.DriveReport.Addresses).toBeDefined();
        });

        it('Scope.DriveReport.Addresses should have initial length 2', function () {
            expect($scope.DriveReport.Addresses.length).toBe(2);
        });

        it('LicensePlate.get should get called', function() {
            expect(licensePlateMock.get).toHaveBeenCalled();
        });





    });
});

