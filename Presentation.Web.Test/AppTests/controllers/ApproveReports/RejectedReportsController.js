describe('ApproveReports', function () {
    beforeEach(module('application'));

    var $controller, modalInstance, $scope, reportMock, orgUnitMock, personMock, bankAccountMock;

    beforeEach(inject(function (_$controller_, _$q_, _$rootScope_) {
        // The injector unwraps the underscores (_) from around the parameter names when matching
        $controller = _$controller_;
        $scope = _$rootScope_.$new();
        $q = _$q_
    }));



    describe('RejectedReportsController', function () {
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
                },
                getAll: function () {
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

            controller = $controller('AcceptedReportsController', { $scope: $scope, $modalInstance: modalInstance, itemId: 2, Report: reportMock, OrgUnit: orgUnitMock, Person: personMock });

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