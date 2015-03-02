angular.module("application").service('Substitute', ["$resource", function ($resource) {
    return $resource("/odata/Substitutes(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false }
    });
}]);