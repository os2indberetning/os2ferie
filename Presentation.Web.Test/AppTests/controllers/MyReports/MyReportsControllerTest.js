describe('MyReports', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope, reportMock;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
        $q = _$q_
    }));



    describe('MyReportsController', function () {
        var controller;

        beforeEach(function () {
            reportMock = {
                get: function () {
                    queryDeferred = $q.defer();
                    return { $promise: queryDeferred.promise };
                }
            }

            controller = $controller('MyReportsController', { $scope: $scope, $modalInstance: modalInstance, itemId: 2, Report: reportMock});

        });

        

        it('activeTab should be pending initally', function () {
            expect($scope.activeTab).toBe("pending");
        });

        it('activeTab should be accepted after calling tabClicked("accepted")', function () {
            $scope.tabClicked("accepted");
            expect($scope.activeTab).toBe("accepted");
        });

        it('activeTab should be rejected after calling tabClicked("rejected")', function () {
            $scope.tabClicked("rejected");
            expect($scope.activeTab).toBe("rejected");
        });

        it('activeTab should be pending after calling tabClicked("pending")', function () {
            $scope.tabClicked("pending");
            expect($scope.activeTab).toBe("pending");
        });

        it('dateOptions.format should be dd/MM/yyyy', function () {
            expect($scope.dateOptions.format).toBe("dd/MM/yyyy");
        });

        it('initial date values should be loaded', function () {
            expect($scope.dateContainer.toDatePending).toBeDefined();
            expect($scope.dateContainer.toDateAccepted).toBeDefined();
            expect($scope.dateContainer.toDateRejected).toBeDefined();
            expect($scope.dateContainer.fromDatePending).toBeDefined();
            expect($scope.dateContainer.fromDateAccepted).toBeDefined();
            expect($scope.dateContainer.fromDateRejected).toBeDefined();
        });


        it('getEndOfDayStamp should return correct timestamp', function () {
            var result = $scope.getEndOfDayStamp("2014-04-25T01:32:21.196Z");
            expect(result).toBe(1398463199);
        });

        it('getStartOfDayStamp should return correct timestamp', function () {
            var result = $scope.getStartOfDayStamp("2014-04-25T01:32:21.196Z");
            expect(result).toBe(1398376800);
        });

        it('PendingGrid url should filter status pending and personId 4 initially', function () {
            $scope.loadPendingReports();
            $scope.gridContainer.pendingGrid = $scope.pendingReports;
            expect($scope.gridContainer.pendingGrid.dataSource.transport.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending' and PersonId eq 4");
        });

        it('acceptedGrid url should filter status accepted and personId 4 initially', function () {
            $scope.loadAcceptedReports();
            $scope.gridContainer.acceptedGrid = $scope.acceptedReports;
            expect($scope.gridContainer.acceptedGrid.dataSource.transport.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted' and PersonId eq 4");
        });

        it('rejectedGrid url should filter status rejected and personId 4 initially', function () {
            $scope.loadRejectedReports();
            $scope.gridContainer.rejectedGrid = $scope.rejectedReports;
            expect($scope.gridContainer.rejectedGrid.dataSource.transport.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected' and PersonId eq 4");
        });

        it('PendingGrid url should be updated with oDataQuery when calling updatePendingReports', function() {
            $scope.gridContainer.pendingGrid = {};
            $scope.gridContainer.pendingGrid.dataSource = {};
            $scope.gridContainer.pendingGrid.dataSource.read = function (){};
            $scope.gridContainer.pendingGrid.dataSource.transport = {};
            $scope.gridContainer.pendingGrid.dataSource.transport.options = {};
            $scope.gridContainer.pendingGrid.dataSource.transport.options.read = {};
            $scope.gridContainer.pendingGrid.dataSource.transport.options.read.url = "";
            $scope.updatePendingReports("query");
            expect($scope.gridContainer.pendingGrid.dataSource.transport.options.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending' and PersonId eq 4 and query");
        });

        it('AcceptedGrid url should be updated with oDataQuery when calling updateAcceptedReports', function () {
            $scope.gridContainer.acceptedGrid = {};
            $scope.gridContainer.acceptedGrid.dataSource = {};
            $scope.gridContainer.acceptedGrid.dataSource.read = function () { };
            $scope.gridContainer.acceptedGrid.dataSource.transport = {};
            $scope.gridContainer.acceptedGrid.dataSource.transport.options = {};
            $scope.gridContainer.acceptedGrid.dataSource.transport.options.read = {};
            $scope.gridContainer.acceptedGrid.dataSource.transport.options.read.url = "";
            $scope.updateAcceptedReports("query");
            expect($scope.gridContainer.acceptedGrid.dataSource.transport.options.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted' and PersonId eq 4 and query");
        });

        it('RejectedGrid url should be updated with oDataQuery when calling updateRejectedGrid', function () {
            $scope.gridContainer.rejectedGrid = {};
            $scope.gridContainer.rejectedGrid.dataSource = {};
            $scope.gridContainer.rejectedGrid.dataSource.read = function () { };
            $scope.gridContainer.rejectedGrid.dataSource.transport = {};
            $scope.gridContainer.rejectedGrid.dataSource.transport.options = {};
            $scope.gridContainer.rejectedGrid.dataSource.transport.options.read = {};
            $scope.gridContainer.rejectedGrid.dataSource.transport.options.read.url = "";
            $scope.updateRejectedReports("query");
            expect($scope.gridContainer.rejectedGrid.dataSource.transport.options.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected' and PersonId eq 4 and query");
        });


    });
});