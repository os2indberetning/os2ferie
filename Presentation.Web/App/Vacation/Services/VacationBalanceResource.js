var app;
(function (app) {
    var vacation;
    (function (vacation) {
        var resources;
        (function (resources) {
            angular.module('app.vacation')
                .factory('VacationBalanceResource', [
                '$resource', function ($resource) {
                    var getAction = {
                        method: "GET",
                        isArray: true,
                        transformResponse: function (res) {
                            return angular.fromJson(res).value;
                        }
                    };
                    return $resource("/odata/VacationBalance(:id)?:query", { id: "@id", query: "@query" }, {
                        get: getAction
                    });
                }
            ]);
        })(resources = vacation.resources || (vacation.resources = {}));
    })(vacation = app.vacation || (app.vacation = {}));
})(app || (app = {}));
