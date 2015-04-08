angular.module("application").service('LicensePlate', ["$resource", function ($resource) {
    return $resource("/odata/LicensePlates(:id)", { id: "@id" }, {
        "get": {
            url: "/odata/LicensePlates?$filter=PersonId eq :id",
            method: "GET", transformResponse: function (data) {
                return angular.fromJson(data).value;
            },
            isArray: true
        },
        "delete": { method: "DELETE" },
        "patch": { method : "PATCH"},
       
    });
}]);