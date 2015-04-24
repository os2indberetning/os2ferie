angular.module("application").service('PersonalAddress', ["$resource", function ($resource) {
    return $resource("/odata/PersonalAddresses(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH" },
        "GetHomeForUser": {
            method: "GET",
            isArray: false,
            url: "/odata/PersonalAddresses/Service.GetHome?personId=:id",
            transformResponse: function(data) {
                return angular.fromJson(data).value[0];
                
            }
        }
    });
}]);