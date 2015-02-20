angular.module("application").service("PersonEmployments", ["$resource", function ($resource) {
    return $resource("/odata/Person(:id)/Employments", { id: "@id" }, {
        "get": {
            method: "GET",
            isArray: true,
            transformResponse: function (data) {
                return angular.fromJson(data).value;
            }
        }
    });
}]);