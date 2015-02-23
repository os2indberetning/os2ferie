﻿angular.module("application").service('Report', ["$resource", function ($resource) {
    return $resource("/odata/DriveReports(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: true },
		"patch" : {method: "PATCH", isArray: true},
		"delete" : {method: "DELETE", isArray: true}
    });
}]);