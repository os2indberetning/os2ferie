angular.module("application").service('Route', ["$resource", function ($resource) {
    return $resource("/odata/PersonalRoutes(:id)?$expand=Points&:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false },
        "delete": {method: "DELETE", isArray: false}
    });
}]);