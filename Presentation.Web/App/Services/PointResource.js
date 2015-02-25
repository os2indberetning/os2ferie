angular.module("application").service('Point', ["$resource", function ($resource) {
    return $resource("/odata/Points(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false }
    });
}]);