angular.module("application").service('Person', ["$resource", "$modal", function ($resource, $modal) {
    return $resource("/odata/Person(:id)", { id: "@id" }, {
        "get": {
            method: "GET", isArray: false, transformResponse: function (data) {
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
        "getAll": {
            method: "GET", isArray: false, url: "/odata/Person", transformResponse: function(data) {
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
            }
        },
        "patch": { method: "PATCH" },
        "getNonAdmins": {
            url: "/odata/Person?$filter=IsAdmin eq false",
            method: "GET", isArray: false, transformResponse: function(data) {
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
            }
        },
        "GetCurrentUser" : {
            url: "/odata/Person/Service.GetCurrentUser?$expand=Employments($expand=AlternativeWorkAddress,OrgUnit($expand=Address))",
            method: "GET",
            transformResponse: function (data) {
                var res = angular.fromJson(data);
                res.IsLeader = (function() {
                    var returnVal = false;
                    angular.forEach(res.Employments, function(value, key) {
                        if (value.IsLeader === true) {
                            returnVal = true;
                        }
                    });
                    return returnVal;
                })();
                return res;
            }
        }
    });
}]);