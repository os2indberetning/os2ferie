angular.module("application").service('DriveReport', ["$resource", function ($resource) {
    return $resource("/odata/DriveReports(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH" }
    });
}]);