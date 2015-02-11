angular.module("application").service('Point', ["$resource", function ($resource) {
    return $resource("/odata/Points(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false }
    });
}]);