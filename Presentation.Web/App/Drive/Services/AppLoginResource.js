angular.module("app.drive").service('AppLogin', ["$resource", function ($resource) {
    return $resource("/odata/AppLogin(:id)", { id: "@id"}, {
        "delete": {method: "DELETE", isArray: false},
        "post": {method: "POST", isArray: false}
    });
}]);

