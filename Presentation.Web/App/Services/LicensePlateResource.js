﻿angular.module("application").service('LicensePlate', ["$resource", function ($resource) {
    return $resource("/odata/LicensePlates(:id)", { id: "@id" }, {
        "get": {
            method: "GET", transformResponse: function (data) {
                return angular.fromJson(data).value[0];
            }
        },
        "delete": { method: "DELETE" }
        
    });
}]);