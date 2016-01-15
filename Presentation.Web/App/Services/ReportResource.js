﻿angular.module("application").service('Report', ["$resource", function ($resource) {
    return $resource("/odata/DriveReports(:id)", { id: "@id", emailText: "@emailText" }, {
        "get": { method: "GET", isArray: true },
        "patch": {
            method: "PATCH",
            isArray: true, 
            url: "/odata/DriveReports(:id)?emailText=:emailText",
            transformRequest: function (data) {
                debugger;
                delete data.emailText;
            },
        },
		"delete" : {method: "DELETE", isArray: true}
    });
}]);