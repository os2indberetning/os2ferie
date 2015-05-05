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
        "getWorkAddresses": {
            method:  "GET",
            isArray: false,
            url: "/odata/Person(:personId)/Employments?$filter=Id eq :employmentId &$expand=OrgUnit($expand=Address)",
            transformResponse: function (data) {
                var res = angular.fromJson(data).value[0].OrgUnit.Address;
                res.PresentationString = "Arbejdsadresse : " + res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town;
                return res;
               
            }
        }
    });
}]);