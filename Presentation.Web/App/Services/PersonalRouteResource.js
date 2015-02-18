angular.module("application").service('Personalroute', ["$resource", function ($resource) {
    return $resource("/odata/PersonalRoutes(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false }
    });
}]);