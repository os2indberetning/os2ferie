angular.module("application").service('PersonalAddress', ["$resource", function ($resource) {
    return $resource("/odata/PersonalAddresses(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH" },
        "GetWorkAndHomeForUser": {
            method: "GET",
            isArray: true,
            url: "/odata/PersonalAddresses/Service.GetHomeAndWork?personId=:id",
            transformResponse: function(data) {
                return angular.fromJson(data).value;
                
            }
        }
    });
}]);