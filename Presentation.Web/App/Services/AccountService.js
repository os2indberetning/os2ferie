angular.module("application").service('Account', ["$resource", function ($resource) {
    return $resource("/odata/PersonalAddresses(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false }
    });
}]);