angular.module("app.drive").service('StandardAddress', ["$resource", function ($resource) {
    return $resource("/odata/Addresses(:id)", { id: "@id" }, {
        "get": { method: "GET", isArray: false },
        "delete": { method: "DELETE", isArray: false },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false },
        "GetStandard": {
            method: "GET",
            url: "/odata/Addresses/Service.GetStandard",
            isArray: true,
            transformResponse: function (data) {
                var res = angular.fromJson(data);
                if (res.error == undefined) {
                    return res.value;
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
        }
    });
}]);

