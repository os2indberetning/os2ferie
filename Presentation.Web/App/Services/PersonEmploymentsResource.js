angular.module("application").service("PersonEmployments", ["$resource", function ($resource) {
    return $resource("/odata/Person(:id)/Employments?$expand=OrgUnit", { id: "@id" }, {
        "get": {
            method: "GET",
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
        },
        "patchEmployment": {
            method: "PATCH",
            isArray: false,
            url: "/odata/Employments(:id)"
        }
    });
}]);