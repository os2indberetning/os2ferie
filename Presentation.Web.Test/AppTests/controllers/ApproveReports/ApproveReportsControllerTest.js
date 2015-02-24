describe('ApproveReports', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope, reportMock, orgUnitMock, personMock, bankAccountMock;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
        $q = _$q_
    }));



    describe('ApproveReportsController', function () {
        var controller;

        beforeEach(function () {
            reportMock = {
                get: function () {
                    queryDeferred = $q.defer();
                    return { $promise: queryDeferred.promise };
                }
            }

            orgUnitMock = {
                get: function () {
                    queryDeferred = $q.defer();
                    return { $promise: queryDeferred.promise };
                }
            }

            personMock = {
                get: function () {
                    queryDeferred = $q.defer();
                    return { $promise: queryDeferred.promise };
                }
            }

            bankAccountMock = {
                get: function () {
                    queryDeferred = $q.defer();
                    return { $promise: queryDeferred.promise };
                }
            }

            controller = $controller('ApproveReportsController', { $scope: $scope, $modalInstance: modalInstance, itemId: 2, Report: reportMock, OrgUnit: orgUnitMock, Person: personMock });

        });

        it('chosenPerson should be undefined initially', function () {
            expect($scope.chosenPerson).toBe(undefined);
        });

        it('chosenPerson should be cleared when calling clearName', function () {
            $scope.clearName();
            expect($scope.chosenPerson).toBe("");
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

        it('PendingGrid url should filter status pending and expand employment initially', function () {
            $scope.loadPendingReports();
            $scope.gridContainer.pendingGrid = $scope.pendingReports;
            expect($scope.gridContainer.pendingGrid.dataSource.transport.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Pending'&$expand=Employment");
        });

        it('AcceptedGrid url should filter status accepted and expand employment initially', function () {
            $scope.loadAcceptedReports();
            $scope.gridContainer.acceptedGrid = $scope.acceptedReports;
            expect($scope.gridContainer.acceptedGrid.dataSource.transport.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Accepted'&$expand=Employment");
        });

        it('RejectedGrid url should filter status rejected and expand employment initially', function () {
            $scope.loadRejectedReports();
            $scope.gridContainer.rejectedGrid = $scope.rejectedReports;
            expect($scope.gridContainer.rejectedGrid.dataSource.transport.read.url).toBe("/odata/DriveReports?$filter=Status eq Core.DomainModel.ReportStatus'Rejected'&$expand=Employment");
        });

        it('getEndOfDayStamp should return correct timestamp', function () {
            var result = $scope.getEndOfDayStamp("2014-04-25T01:32:21.196Z");
            expect(result).toBe(1398463199);
        });

        it('getStartOfDayStamp should return correct timestamp', function () {
            var result = $scope.getStartOfDayStamp("2014-04-25T01:32:21.196Z");
            expect(result).toBe(1398376800);
        });


        it('filterReportsByLeaderOrg should return 5 reports with leaderOrgId 1', function () {
            var orgs = {};
            orgs.value = [];

            var data = {};
            data.value = [];

            orgs.value.push({
                Id: 1,
                OrgId: 1,
                ParentId: null,
                Level: 1
            });

            orgs.value.push({
                Id: 2,
                OrgId: 2,
                ParentId: 1,
                Level: 2
            });

            orgs.value.push({
                Id: 3,
                OrgId: 3,
                ParentId: 2,
                Level: 3
            });

            orgs.value.push({
                Id: 4,
                OrgId: 4,
                ParentId: null,
                Level: 1
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 3
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 4
                },
            });

            var resultSet = $scope.filterReportsByLeaderOrg(orgs, data, 1);

            expect(resultSet.length).toBe(5);


        });

        it('filterReportsByLeaderOrg should return 1 report with leaderOrgId 4', function () {
            var orgs = {};
            orgs.value = [];

            var data = {};
            data.value = [];

            orgs.value.push({
                Id: 1,
                OrgId: 1,
                ParentId: null,
                Level: 1
            });

            orgs.value.push({
                Id: 2,
                OrgId: 2,
                ParentId: 1,
                Level: 2
            });

            orgs.value.push({
                Id: 3,
                OrgId: 3,
                ParentId: 2,
                Level: 3
            });

            orgs.value.push({
                Id: 4,
                OrgId: 4,
                ParentId: null,
                Level: 1
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 3
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 4
                },
            });

            var resultSet = $scope.filterReportsByLeaderOrg(orgs, data, 4);

            expect(resultSet.length).toBe(1);


        });


        it('filterReportsByLeaderOrg should return 3 reports with leaderOrgId 2', function () {
            var orgs = {};
            orgs.value = [];

            var data = {};
            data.value = [];

            orgs.value.push({
                Id: 1,
                OrgId: 1,
                ParentId: null,
                Level: 1
            });

            orgs.value.push({
                Id: 2,
                OrgId: 2,
                ParentId: 1,
                Level: 2
            });

            orgs.value.push({
                Id: 3,
                OrgId: 3,
                ParentId: 2,
                Level: 3
            });

            orgs.value.push({
                Id: 4,
                OrgId: 4,
                ParentId: null,
                Level: 1
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 3
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 4
                },
            });

            var resultSet = $scope.filterReportsByLeaderOrg(orgs, data, 2);

            expect(resultSet.length).toBe(3);


        });

        it('filterReportsByLeaderOrg should return 1 report with leaderOrgId 3', function () {
            var orgs = {};
            orgs.value = [];

            var data = {};
            data.value = [];

            orgs.value.push({
                Id: 1,
                OrgId: 1,
                ParentId: null,
                Level: 1
            });

            orgs.value.push({
                Id: 2,
                OrgId: 2,
                ParentId: 1,
                Level: 2
            });

            orgs.value.push({
                Id: 3,
                OrgId: 3,
                ParentId: 2,
                Level: 3
            });

            orgs.value.push({
                Id: 4,
                OrgId: 4,
                ParentId: null,
                Level: 1
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 3
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 4
                },
            });

            var resultSet = $scope.filterReportsByLeaderOrg(orgs, data, 3);

            expect(resultSet.length).toBe(1);


        });

        it('filterReportsByLeaderOrg should return 0 reports with leaderOrgId 5', function () {
            var orgs = {};
            orgs.value = [];

            var data = {};
            data.value = [];

            orgs.value.push({
                Id: 1,
                OrgId: 1,
                ParentId: null,
                Level: 1
            });

            orgs.value.push({
                Id: 2,
                OrgId: 2,
                ParentId: 1,
                Level: 2
            });

            orgs.value.push({
                Id: 3,
                OrgId: 3,
                ParentId: 2,
                Level: 3
            });

            orgs.value.push({
                Id: 4,
                OrgId: 4,
                ParentId: null,
                Level: 1
            });

            orgs.value.push({
                Id: 5,
                OrgId: 5,
                ParentId: null,
                Level: 1
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 1
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 2
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 3
                },
            });

            data.value.push({
                Employment: {
                    OrgUnitId: 4
                },
            });

            var resultSet = $scope.filterReportsByLeaderOrg(orgs, data, 5);

            expect(resultSet.length).toBe(0);


        });
    });
});