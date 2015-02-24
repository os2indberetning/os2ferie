describe("MyReportsController", function () {
    beforeEach(module("application"));

    describe("activeTab", function() {
        var scope,
            controller,
            report;

        beforeEach(inject(function($rootScope, $controller, Report) {
            scope = $rootScope.$new();
            controller = $controller;
            report = Report;
        }));

        it("activeTab should be pending by default", function() {
            controller("MyReportsController", { $scope: scope });

            expect(scope.activeTab).toBe("pending");
        });

        it("activeTab should be accepted after tabClicked", function() {
            controller("MyReportsController", { $scope: scope });
            scope.tabClicked("accepted");
            expect(scope.activeTab).toBe("accepted");
        })

        it("activeTab should be rejected after tabClicked", function() {
            controller("MyReportsController", { $scope: scope });
            scope.tabClicked("rejected");
            expect(scope.activeTab).toBe("rejected");
        })

    });
});