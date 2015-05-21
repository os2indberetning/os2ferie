angular.module("application").service('EmailNotification', ["$resource", function ($resource) {
    return $resource("/odata/MailNotifications(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false, transformResponse: function(data) {
            var res = angular.fromJson(data);
            if (res.error == undefined) {
                return res.value[0];
            }

            var modalInstance = $modal.open({
                templateUrl: '/App/Services/Error/ServiceError.html',
                controller: "ServiceErrorController",
                backdrop: "static",
                resolve: {
                    errorMsg: function () {
                        return res.error.innererror.message;
                    }
                }
            });
            return res;
        }},
        "getAll": {
            method: "GET", isArray: false
        },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false }
    });
}]);