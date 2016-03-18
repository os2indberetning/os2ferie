angular.module("app.drive").service('Route', ["$resource", function ($resource) {
    return $resource("/odata/PersonalRoutes(:id)?$expand=Points&:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "getSingle": {
            method: "GET",
            isArray: false,
            url: "/odata/PersonalRoutes?$expand=Points &$filter=Id eq :id",
            transformResponse: function (data) {
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
            }
        },
        "patch": { method: "PATCH", isArray: false },
        "post": {
            method: "POST",
            isArray: false,
            url: "/odata/PersonalRoutes"
        },
        "delete": { method: "DELETE", isArray: false }
    });
}]);