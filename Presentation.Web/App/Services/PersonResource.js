angular.module("application").service('Person', ["$resource", "$modal", function ($resource, $modal) {
    return $resource("/odata/Person(:id)", { id: "@id" }, {
        "get": {
            method: "GET", isArray: false, transformResponse: function (data) {
                var res = angular.fromJson(data);
                return res;
            }
        },
        "getAll": {
            method: "GET", isArray: false, url: "/odata/Person?:query", transformResponse: function(data) {
                var res = angular.fromJson(data);
                return res;
            }
        },
        "patch": { method: "PATCH" },
        "getNonAdmins": {
            url: "/odata/Person?$filter=IsAdmin eq false and IsActive eq true &$select=Id,FullName",
            method: "GET", isArray: false, transformResponse: function(data) {
                var res = angular.fromJson(data);
                return res;
            }
        },
        "GetCurrentUser" : {
            url: "/odata/Person/Service.GetCurrentUser?$select=Id,IsSubstitute,RecieveMail,IsAdmin,FullName,Mail,DistanceFromHomeToBorder &$expand=PersonalRoutes($expand=Points),LicensePlates,Employments($expand=AlternativeWorkAddress,OrgUnit($select=Id,LongDescription,HasAccessToFourKmRule; $expand=Address); $select=Id,Position,IsLeader,HomeWorkDistance,WorkDistanceOverride, AlternativeWorkAddressId)",
            method: "GET",
            transformResponse: function (data) {
                var res = angular.fromJson(data);

                if (res.error == undefined) {
                    // If the request did not yield an error, then finish the request and return it.
                    res.IsLeader = (function () {
                        var returnVal = false;
                        angular.forEach(res.Employments, function (value, key) {
                            if (value.IsLeader === true) {
                                returnVal = true;
                            }
                        });
                        return returnVal;
                    })();
                    return res;
                }

                // If there was an error then open modal.
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
        "GetLeaders" : {
            url: "/odata/Employments?$filter=IsLeader eq true&$select=Person &$expand=Person($select=Id,FullName)",
            method: "GET",
            isArray: true,
            transformResponse: function (data) {
                var map = {};
                var result = [];
                var leaders = angular.fromJson(data).value;

                // Remove duplicate values.
                for (var i = 0; i < leaders.length; i++) {
                    if (map[leaders[i].Person.FullName] == undefined) {
                        result.push(leaders[i].Person);
                        map[leaders[i].Person.FullName] = leaders[i].Person;
                    }
                }

                return result;
            }
        }
    });
}]);