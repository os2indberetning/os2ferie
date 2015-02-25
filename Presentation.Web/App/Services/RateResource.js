angular.module("application").service("Rate", ["$resource", function ($resource) {
    return $resource("/odata/Rates(:id)", { id: "@id" }, {
        "get": {
            method: "GET",
            isArray: true,
            transformResponse: function(data) {
                return angular.fromJson(data).value;
            }
        },
        "ThisYearsRates": {
            method: "GET",
            isArray: true,
            transformResponse: function(data) {
                return angular.fromJson(data).value;
            },
            url: "/odata/Rates/RateService.ThisYearsRates"
        }
    });
}]);