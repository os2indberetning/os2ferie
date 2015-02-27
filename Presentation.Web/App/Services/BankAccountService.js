angular.module("application").service('BankAccount', ["$resource", function ($resource) {
    return $resource("/odata/BankAccounts(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH" },
        "post": {method: "POST"}
    });
}]);