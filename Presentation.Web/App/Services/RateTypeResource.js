angular.module("application").service("RateType", ["$resource", function ($resource) {
    return $resource("/odata/RateTypes(:id)", { id: "@id" }, {
        "get": {
            method: "GET",
            isArray: true,
            transformResponse: function (data) {
                return angular.fromJson(data).value;
            }
        },
       "post": {method: "POST"}
    });
}]);