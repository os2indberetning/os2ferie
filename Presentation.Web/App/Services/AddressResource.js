angular.module("application").service('Address', ["$resource", function ($resource) {
    return $resource("/odata/PersonalAddresses(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false },
        "delete": { method: "DELETE", isArray: false },
        "getAlternativeHome": {
            method: "GET",
            url: "/odata/PersonalAddresses(:id)/AlternativeHome?:query",
            isArray: true,
            transformResponse: function (data) {
                return angular.fromJson(data).value;
            }
        },
        "getAlternativeWork": {
            method: "GET",
            url: "/odata/PersonalAddresses(:id)/AlternativeWork?:query",
            isArray: true,
            transformResponse: function (data) {
                return angular.fromJson(data).value;
            }
        }
    });
}]);