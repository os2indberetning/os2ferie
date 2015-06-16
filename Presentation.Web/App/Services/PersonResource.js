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
            url: "/odata/Person?$filter=IsAdmin eq false &$select=Id,FullName",
            method: "GET", isArray: false, transformResponse: function(data) {
                var res = angular.fromJson(data);
                return res;
            }
        },
        "GetCurrentUser" : {
            url: "/odata/Person/Service.GetCurrentUser?$select=Id,IsSubstitute,RecieveMail,IsAdmin,FullName,Mail,DistanceFromHomeToBorder &$expand=PersonalRoutes($expand=Points),LicensePlates,Employments($select=Id,IsLeader,HomeWorkDistance,Position,WorkDistanceOverride,AlternativeWorkAddressId;$expand=AlternativeWorkAddress;$expand=OrgUnit($select=Id,LongDescription,HasAccessToFourKmRule;$expand=Address))",
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
        },
        "GetLeaders" : {
            url: "/odata/Employments?$filter=IsLeader eq true&$select=Person &$expand=Person($select=Id,FullName)",
            method: "GET",
            isArray: true,
            transformResponse: function (data) {
                var result = [];
                var leaders = angular.fromJson(data).value;
                angular.forEach(leaders, function (leader, key) {
                    result.push(leader.Person);
                });
                return result;
            }
        }
    });
}]);