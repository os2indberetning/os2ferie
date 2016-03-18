angular.module("app.drive").service('Address', ["$resource", "$modal", function ($resource, $modal) {
    return $resource("/odata/PersonalAddresses(:id)?:query", { id: "@id", query: "@query" }, {
        "get": { method: "GET", isArray: false },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false },
        "delete": { method: "DELETE", isArray: false },
        "getAlternativeHome": {
            method: "GET",
            url: "/odata/PersonalAddresses(:id)/AlternativeHome?:query",
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
        "getAlternativeWork": {
            method: "GET",
            url: "/odata/PersonalAddresses(:id)/AlternativeWork?:query",
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
        "setCoordinatesOnAddress": {
            method: "POST",
            url: "/odata/Addresses/Service.SetCoordinatesOnAddress",
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
        "setCoordinatesOnAddressList": {
            method: "POST",
            url: "/api/Coordinate/SetCoordinatesOnAddressList",
            isArray: true,
            transformResponse: function (data) {
                var res = angular.fromJson(data);
                return res;
            }
        },
        "GetPersonalAndStandard": {
            method: "GET",
            url: "/odata/Addresses/Service.GetPersonalAndStandard?personId=:personId",
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
        "getMapStart": {
            method: "GET",
            url: "/odata/Addresses/Service.GetMapStart",
            isArray: true,
            transformResponse: function (data) {
                var res = angular.fromJson(data);
                if (res.error == undefined) {
                    var resArray = [];
                    resArray.push({ name: res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town, lat: res.Latitude, lng: res.Longitude });
                    resArray.push({ name: res.StreetName + " " + res.StreetNumber + ", " + res.ZipCode + " " + res.Town, lat: res.Latitude, lng: res.Longitude });
                    return resArray;
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
        "AttemptCleanCachedAddress": {
            method: "POST",
            url: "/odata/Addresses/Service.AttemptCleanCachedAddress"
        },
        "GetAutoCompleteDataForCachedAddress": {
            method: "GET",
            url: "/odata/Addresses/Service.GetCachedAddresses?$select=Description,DirtyString"
        }
    });
}]);