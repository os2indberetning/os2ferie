angular.module("application").service('EmailNotification', ["$resource", function ($resource) {
    return $resource("/odata/MailNotifications(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false, transformResponse: function(data) {
            return angular.fromJson(data).value[0];
        }},
        "getAll": {
            method: "GET", isArray: false
        },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false }
    });
}]);