﻿angular.module("application").service('DriveReport', ["$resource", function ($resource) {
    return $resource("/odata/DriveReports(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH" },
        "getLatest": {
            method: "GET",
            isArray: false,
            url: "/odata/DriveReports?$filter=PersonId eq :id &$top=1&$orderby=CreatedDateTimestamp desc &$expand=Employment",
            transformResponse: function (data) {
                return angular.fromJson(data).value[0];
            }
        },
        "getWithPoints": {
            method: "GET",
            isArray: false,
            url: "/odata/DriveReports?$filter=Id eq :id &$expand=DriveReportPoints",
            transformResponse: function(data) {
                return angular.fromJson(data).value[0];
            }
        }
    });
}]);