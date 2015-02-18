angular.module("application").service('Route', ["$resource", function ($resource) {
    return $resource("/odata/PersonalRoutes(:id)?$expand=Points", { id: "@id" }, {
        "get": { method: "GET", isArray: false }
    });
}]);