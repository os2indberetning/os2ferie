angular.module("app.vacation").service('VacationBalance', ["$resource", function ($resource) {
    return $resource("/odata/VacationBalance(:id)?:query", { id: "@id", query: "@query"}, {
        "get": {
            method: "GET", isArray: true, transformResponse: function (res) {
            return angular.fromJson(res).value;
        } },
    });
}]);