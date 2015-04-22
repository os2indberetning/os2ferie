angular.module("application").service("Rate", ["$resource", function ($resource) {
    return $resource("/odata/Rates(:id)", { id: "@id" }, {
        "get": {
            method: "GET",
            isArray: true,
            transformResponse: function(data) {
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
        },
        "ThisYearsRates": {
            method: "GET",
            isArray: true,
            transformResponse: function(data) {
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
            },
            url: "/odata/Rates/Service.ThisYearsRates?$expand=Type"
        },
        "post": {method: "POST"}
    });
}]);