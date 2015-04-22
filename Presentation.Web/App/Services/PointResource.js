angular.module("application").service('Point', ["$resource", function ($resource) {
    return $resource("/odata/Points(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false, transformResponse: function(data) {
            var res = angular.fromJson(data);
            if (res.error == undefined) {
                return res;
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
        } },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false }
    });
}]);