﻿angular.module("app.drive").service('Report', ["$resource", function ($resource) {
    return $resource("/odata/DriveReports(:id)", { id: "@id", emailText: "@emailText" }, {
        "get": { method: "GET", isArray: true },
        "patch": {
            method: "PATCH",
            isArray: true, 
            url: "/odata/DriveReports(:id)?emailText=:emailText",
        },
        "getOwner": { 
            url: "/odata/DriveReports?$filter=Id eq :id&$select=Person&$expand=Person",
            method: "GET", 
            isArray: false, 
            transformResponse: function(data){
                var res = angular.fromJson(data).value[0].Person;
                return res; 
            } 
        },
		"delete" : {method: "DELETE", isArray: true}
    });
}]);