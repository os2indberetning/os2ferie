angular.module("application").service('StandardAddress', ["$resource", function ($resource) {
    return $resource("/odata/Addresses(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false },
        "delete": { method: "DELETE", isArray: false },
        "patch": { method: "PATCH", isArray: false },
        "post": {method: "POST", isArray: false}
    });
}]);

