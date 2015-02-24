angular.module("application").service('Person', ["$resource", function ($resource) {
    return $resource("/odata/Person(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false, transformResponse: function(data) {
            return angular.fromJson(data).value[0];
        }},
        "patch": { method: "PATCH" }
    });
}]);