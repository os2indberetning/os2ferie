angular.module("application").service('Route', ["$resource", function ($resource) {
    return $resource("/odata/PersonalRoutes(:id)?$expand=Points&:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "getSingle": {
            method: "GET",
            isArray: false,
            url: "/odata/PersonalRoutes?$expand=Points &$filter=Id eq :id",
            transformResponse: function (data) {
                return angular.fromJson(data).value[0];
            }
        },
        "patch": { method: "PATCH", isArray: false },
        "post": {
            method: "POST",
            isArray: false,
            url: "/odata/PersonalRoutes"
        },
        "delete": { method: "DELETE", isArray: false }
    });
}]);