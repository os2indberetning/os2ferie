angular.module("application").service('OrgUnit', ["$resource", function ($resource) {
    return $resource("/odata/OrgUnits(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false }
    });
}]);
