angular.module("application").service('Personalroute', ["$resource", function ($resource) {
    return $resource("/odata/PersonalRoutes(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "delete": {method: "DELETE", isArray: false}
    });
}]);