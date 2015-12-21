angular.module("application").service('OrgUnit', ["$resource", function ($resource) {
    return $resource("/odata/OrgUnits(:id)?:query", { id: "@id"}, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH", isArray: false },
        "getWhereUserIsLeader": {
            method: "GET",
            isArray: true,
            url: "/odata/OrgUnits/Service.GetWhereUserIsResponsible?personId=:id",
            transformResponse: function (data) {
                return angular.fromJson(data).value;
            }
        },
        "getLeaderOfOrg": {
            method: "GET",
            isArray: false,
            url: "/odata/OrgUnits/Service.GetLeaderOfOrg?orgId=:id",
            transformResponse: function (data) {
                var result = angular.fromJson(data);
                return result;
            }
        }
    });
}]);

