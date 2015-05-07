angular.module("application").service('PersonalAddress', ["$resource", function ($resource) {
    return $resource("/odata/PersonalAddresses(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH" },
        "delete": { method: "DELETE" },
        "post": { method: "POST" },
        "GetHomeForUser": {
            method: "GET",
            isArray: false,
            url: "/odata/PersonalAddresses/Service.GetHome?personId=:id",
            transformResponse: function (data) {
                return angular.fromJson(data).value[0];

            }
        },
        "GetRealHomeForUser": {
            method: "GET",
            isArray: false,
            url: "/odata/PersonalAddresses/Service.GetRealHome?personId=:id",
            transformResponse: function (data) {
                var res = angular.fromJson(data);
                return res;

            }
        },
        "GetAlternativeHomeForUser": {
            method: "GET",
            isArray: false,
            url: "/odata/PersonalAddresses/Service.GetAlternativeHome?personId=:id",
            transformResponse: function (data) {
                var res = angular.fromJson(data);
                return res;

            }
        }
    });
}]);