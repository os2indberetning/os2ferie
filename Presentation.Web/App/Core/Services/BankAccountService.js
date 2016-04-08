angular.module("app.core").service('BankAccount', ["$resource", function ($resource) {
    return $resource("/odata/BankAccounts(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH" },
        "post": { method: "POST" },
        "getByAccount" : {
            method: "GET",
            url: "/odata/BankAccounts?$filter=Number eq ':account'",
            transformResponse: function (data) {
                var res = angular.fromJson(data).value[0];
                return res;
            }
        }
    });
}]);