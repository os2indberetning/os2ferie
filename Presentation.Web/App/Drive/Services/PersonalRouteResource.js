angular.module("app.drive").service('PersonalRoute', ["$resource", function ($resource) {
    return $resource("/odata/PersonalRoutes(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "delete": { method: "DELETE", isArray: false },
        "getForUser": {
            method: "GET",
            isArray: true,
            url: "/odata/PersonalRoutes?$filter=PersonId eq :id &$expand=Points",
            transformResponse: function(res) {
                return angular.fromJson(res).value;
            }
        },
    });
}]);